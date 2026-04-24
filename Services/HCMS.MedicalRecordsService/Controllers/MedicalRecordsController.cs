using HCMS.MedicalRecordsService.Domain.DTOs;
using HCMS.MedicalRecordsService.Domain.Entities;
using HCMS.MedicalRecordsService.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

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
            var bucket = _context.FileBucket;

            using var stream = file.OpenReadStream();

            var fileId = await bucket.UploadFromStreamAsync(
                file.FileName,
                stream);

            var update = Builders<MedicalRecord>.Update.Push(r => r.Files,
                new FileReference
                {
                    Id = fileId,
                    FileName = file.FileName,
                    ContentType = file.ContentType
                });

            await _context.Records.UpdateOneAsync(
                r => r.Id == MongoDB.Bson.ObjectId.Parse(recordId),
                update);
            return Ok(fileId);
        }

        [HttpGet("files/{fileId}")]
        public async Task<IActionResult> DownloadFile(string fileId)
        {
            var bucket = _context.FileBucket;

            var stream = new MemoryStream();

            await bucket.DownloadToStreamAsync(MongoDB.Bson.ObjectId.Parse(fileId), stream);

            stream.Position = 0;

            return File(stream, "application/octet-stream");
        }
    }
}
