using AppointmentServiceGrpc;
using HCMS.MedicalRecordsService.Domain.DTOs;
using HCMS.MedicalRecordsService.Domain.Entities;
using HCMS.MedicalRecordsService.Infrastructure.Auth;
using HCMS.MedicalRecordsService.Infrastructure.Core;
using HCMS.MedicalRecordsService.Infrastructure.Kafka.Events;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System.Text.Json;

namespace HCMS.MedicalRecordsService.Controllers
{
    [ApiController]
    [Route("api/records")]
    public class MedicalRecordsController(ServiceDbContext context, AppointmentExistence.AppointmentExistenceClient grpcClient) : ControllerBase
    {
        private readonly ServiceDbContext _context = context;
        private readonly AppointmentExistence.AppointmentExistenceClient _grpcClient = grpcClient;

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateRecord(CreateRecordDto dto)
        {
            var user = new UserContext(User);

            if (!user.IsDoctor)
                return Forbid();

            var grpcResponse = await _grpcClient.GetAppointmentAsync(
            new GetAppointmentRequest
            {
                AppointmentId = dto.AppointmentId.ToString()
            });

            if (grpcResponse.Status == "Canceled")
                return BadRequest("Appointment is cancelled");

            if (grpcResponse.DoctorId != dto.DoctorId.ToString())
                return BadRequest("Doctor not assigned to this appointment");

            if (grpcResponse.PatientId != dto.PatientId.ToString())
                return BadRequest("Patient mismatch");

            var record = new MedicalRecord
            {
                Id = ObjectId.GenerateNewId(),
                PatientId = dto.PatientId,
                DoctorId = user.UserId,
                AppointmentId = dto.AppointmentId,
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Records.InsertOneAsync(record);

            var eventMessage = new MedicalRecordCreatedEvent(
                record.Id.ToString(),
                record.AppointmentId,
                record.PatientId,
                record.DoctorId);

            var outboxMessage = new OutboxMessage
            {
                Id = ObjectId.GenerateNewId(),
                Type = "medical-record-created",
                Payload = JsonSerializer.Serialize(eventMessage),
                OccurredOn = DateTime.UtcNow,
                Processed = false
            };

            await _context.OutboxMessages.InsertOneAsync(outboxMessage);

            return Ok(record);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(string id, [FromServices] RecordAccessService accessService)
        {
            var record = await _context.Records
                .Find(r => r.Id == ObjectId.Parse(id))
                .FirstOrDefaultAsync();

            if (record == null)
                return NotFound();

            var user = new UserContext(User);

            if (!accessService.CanAccess(user, record))
                return Forbid();

            return Ok(record);
        }

        [HttpPost("{recordId}/files")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UploadFile(string recordId, IFormFile file, [FromServices] RecordAccessService accessService)
        {
            var objectId = ObjectId.Parse(recordId);

            var record = await _context.Records.Find(r => r.Id == ObjectId.Parse(recordId))
                                               .FirstOrDefaultAsync();

            if (record == null)
                return NotFound();

            var user = new UserContext(User);

            if (!accessService.CanAccess(user, record))
                return Forbid();

            using var stream = file.OpenReadStream();

            var bucket = _context.FileBucket;

            var fileId = await bucket.UploadFromStreamAsync(
                file.FileName,
                stream,
                new GridFSUploadOptions
                {
                    Metadata = new BsonDocument
                    {
                        { "contentType", file.ContentType }
                    }
                });

            var update = Builders<MedicalRecord>.Update.Push(r => r.Files,
                new FileReference
                {
                    Id = fileId,
                    FileName = file.FileName,
                    ContentType = file.ContentType
                });

            await _context.Records.UpdateOneAsync(
                r => r.Id == objectId,
                update);
            return Ok(fileId);
        }

        [HttpGet("files/{fileId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status206PartialContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status416RequestedRangeNotSatisfiable)]
        public async Task<IActionResult> DownloadFile(string fileId, [FromServices] RecordAccessService accessService)
        {
            var bucket = _context.FileBucket;

            var objectId = ObjectId.Parse(fileId);
            var filter = Builders<GridFSFileInfo>.Filter.Eq("_id", objectId);
            var query = await bucket.FindAsync(filter);
            var fileInfo = await query.FirstOrDefaultAsync();

            if (fileInfo == null)
                return NotFound();

            var record = await _context.Records.Find(r => r.Files.Any(f => f.Id == objectId)).FirstOrDefaultAsync();

            if (record == null)
                return NotFound();

            var user = new UserContext(User);

            if (!accessService.CanAccess(user, record))
                return Forbid();

            var stream = await bucket.OpenDownloadStreamAsync(objectId);
            var contentType = fileInfo.Metadata?["contentType"]?.AsString ?? "application/octet-stream";

            return File(stream, contentType, fileInfo.Filename, enableRangeProcessing: true);
        }
    }
}
