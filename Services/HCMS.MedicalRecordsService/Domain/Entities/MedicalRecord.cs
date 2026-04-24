using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HCMS.MedicalRecordsService.Domain.Entities
{
    public class MedicalRecord
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public Guid AppointmentId { get; set; }
        public string Notes { get; set; }
        public List<FileReference> Files { get; set; } = [];
        public DateTime CreatedAt { get; set; }

    }
}
