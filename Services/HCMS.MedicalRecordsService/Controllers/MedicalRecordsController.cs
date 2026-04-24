using HCMS.MedicalRecordsService.Domain.DTOs;
using HCMS.MedicalRecordsService.Domain.Entities;
using HCMS.MedicalRecordsService.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HCMS.MedicalRecordsService.Controllers
{
    [ApiController]
    [Route("api/records")]
    public class MedicalRecordsController(ServiceDbContext context) : ControllerBase
    {
        private readonly ServiceDbContext _context = context;

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateRecord(CreateRecordDto dto)
        {
            var record = new MedicalRecord
            {
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                AppointmentId = dto.AppointmentId,
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Records.InsertOneAsync(record);
            return Ok(record);
        }

        [HttpPost("{recordId}/files")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UploadFile(string recordId, IFormFile file)
        {
            var objectId = ObjectId.Parse(recordId);

            var record = await _context.Records.FindAsync(r => r.Id == objectId);

            if (record == null)
                return NotFound();

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
        public async Task<IActionResult> DownloadFile(string fileId)
        {
            var bucket = _context.FileBucket;

            var objectId = ObjectId.Parse(fileId);
            var filter = Builders<GridFSFileInfo>.Filter.Eq("_id", objectId);
            var fileInfo = await bucket.Find(filter).FirstOrDefaultAsync();

            if (fileInfo == null) 
                return NotFound();

            var stream = await bucket.OpenDownloadStreamAsync(objectId);
            var contentType = fileInfo.Metadata?["contentType"]?.AsString ?? "application/octet-stream";

            return File(stream, contentType, fileInfo.Filename, enableRangeProcessing: true);
        }
    }
}
