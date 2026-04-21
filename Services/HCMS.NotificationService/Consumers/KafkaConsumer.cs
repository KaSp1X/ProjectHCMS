using Confluent.Kafka;
using HCMS.NotificationService.Events;
using HCMS.NotificationService.Services;
using System.Text.Json;

namespace HCMS.NotificationService.Consumers
{
    public class KafkaConsumer(IConfiguration config, IServiceProvider provider) : BackgroundService
    {
        private readonly IConfiguration _config = config;
        private readonly IServiceProvider _provider = provider;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = _config["Kafka:BootstrapServers"],
                GroupId = "notification-service",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
            consumer.Subscribe("appointment-created");

            while (!stoppingToken.IsCancellationRequested)
            {
                var result = consumer.Consume(stoppingToken);

                using var scope = _provider.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<NotificationHandler>();
                var message = JsonSerializer.Deserialize<AppointmentCreatedEvent>(result.Message.Value);

                try
                {
                    await handler.Handle(message);
                    consumer.Commit(result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
    }
}
