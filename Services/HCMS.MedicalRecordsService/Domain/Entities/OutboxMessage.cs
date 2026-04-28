using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HCMS.MedicalRecordsService.Domain.Entities
{
    public class OutboxMessage
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Type { get; set; }
        public string Payload { get; set; }
        public DateTime OccurredOn { get; set; }
        public bool Processed { get; set; }
    }
}
