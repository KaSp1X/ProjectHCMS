using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HCMS.MedicalRecordsService.Domain.Entities
{
    public class FileReference
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
    }
}
