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
            consumer.Subscribe(["appointment-created", "appointment-canceled", "appointment-completed", "medical-record-created"]);

            while (!stoppingToken.IsCancellationRequested)
            {
                var result = consumer.Consume(stoppingToken);

                using var scope = _provider.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<NotificationHandler>();

                try
                {
                    if (result.Topic == "appointment-created")
                    {
                        var message = JsonSerializer.Deserialize<AppointmentCreatedEvent>(result.Message.Value);
                        await handler.Handle(message);
                    } 
                    else if (result.Topic == "appointment-canceled")
                    {
                        var message = JsonSerializer.Deserialize<AppointmentCanceledEvent>(result.Message.Value);
                        await handler.Handle(message);
                    }
                    else if (result.Topic == "appointment-completed")
                    {
                        var message = JsonSerializer.Deserialize<AppointmentCompletedEvent>(result.Message.Value);
                        await handler.Handle(message);
                    }
                    else if (result.Topic == "medical-record-created")
                    {
                        var message = JsonSerializer.Deserialize<MedicalRecordCreatedEvent>(result.Message.Value);
                        await handler.Handle(message);
                    }

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
