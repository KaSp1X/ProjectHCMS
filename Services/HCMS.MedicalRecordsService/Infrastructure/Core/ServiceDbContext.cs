using HCMS.MedicalRecordsService.Domain.Entities;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace HCMS.MedicalRecordsService.Infrastructure.Core
{
    public class ServiceDbContext
    {
        private readonly IMongoDatabase _db;

        public ServiceDbContext(IConfiguration config)
        {
            var client = new MongoClient(config["Mongo:ConnectionString"]);
            _db = client.GetDatabase(config["Mongo:Database"]);
        }

        public IMongoCollection<MedicalRecord> Records => _db.GetCollection<MedicalRecord>("records");
        public IMongoCollection<OutboxMessage> OutboxMessages => _db.GetCollection<OutboxMessage>("outbox");

        public GridFSBucket FileBucket => new(_db);
    }
}
