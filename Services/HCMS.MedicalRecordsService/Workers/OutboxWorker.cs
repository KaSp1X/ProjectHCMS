using HCMS.MedicalRecordsService.Domain.Entities;
using HCMS.MedicalRecordsService.Infrastructure.Core;
using HCMS.MedicalRecordsService.Infrastructure.Kafka;
using MongoDB.Driver;

namespace HCMS.MedicalRecordsService.Workers
{
    public class OutboxWorker(IServiceProvider provider) : BackgroundService
    {
        private readonly IServiceProvider _provider = provider;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _provider.CreateScope();

                var db = scope.ServiceProvider.GetRequiredService<ServiceDbContext>();
                var producer = scope.ServiceProvider.GetRequiredService<KafkaProducer>();

                var messages = await db.OutboxMessages
                    .Find(x => !x.Processed)
                    .Limit(10)
                    .ToListAsync(stoppingToken);

                foreach (var msg in messages)
                {
                    await producer.ProduceAsync(msg.Type, msg.Payload);

                    var update = Builders<OutboxMessage>.Update
                        .Set(x => x.Processed, true);

                    await db.OutboxMessages.UpdateOneAsync(
                        x => x.Id == msg.Id,
                        update,
                        cancellationToken: stoppingToken);
                }

                await Task.Delay(2000, stoppingToken);
            }
        }
    }
}
