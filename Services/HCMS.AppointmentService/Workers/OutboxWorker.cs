using HCMS.AppointmentService.Infrastructure.Core;
using HCMS.AppointmentService.Infrastructure.Kafka;
using Microsoft.EntityFrameworkCore;

namespace HCMS.AppointmentService.Workers
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
                    .Where(x => !x.Processed)
                    .Take(10)
                    .ToListAsync(stoppingToken);

                foreach (var msg in messages)
                {
                    await producer.ProduceAsync("appointment-created", msg.Payload);

                    msg.Processed = true;
                }

                await db.SaveChangesAsync(stoppingToken);

                await Task.Delay(2000, stoppingToken);
            }
        }
    }
}