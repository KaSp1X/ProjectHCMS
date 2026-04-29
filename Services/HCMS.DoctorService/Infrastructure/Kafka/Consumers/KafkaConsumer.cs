using Confluent.Kafka;
using HCMS.DoctorService.Domain.Entities;
using HCMS.DoctorService.Infrastructure.Core;
using HCMS.DoctorService.Infrastructure.Kafka.Events;
using System.Text.Json;

namespace HCMS.DoctorService.Infrastructure.Kafka.Consumers
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
                GroupId = "doctor-service",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
            consumer.Subscribe(["appointment-created", "appointment-canceled", "appointment-completed"]);

            while (!stoppingToken.IsCancellationRequested)
            {
                var result = consumer.Consume(stoppingToken);

                using var scope = _provider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ServiceDbContext>();

                if (result.Topic == "appointment-created")
                {
                    var evt = JsonSerializer.Deserialize<AppointmentCreatedEvent>(result.Message.Value);

                    var entity = new BookedAppointment
                    {
                        Id = Guid.NewGuid(),
                        AppointmentId = evt.AppointmentId,
                        PatientId = evt.PatientId,
                        DoctorId = evt.DoctorId,
                        StartTime = evt.StartTime,
                        EndTime = evt.EndTime
                    };

                    db.BookedAppointments.Add(entity);
                }
                else if (result.Topic == "appointment-canceled" || result.Topic == "appointment-completed")
                {
                    var evt = JsonSerializer.Deserialize<AppointmentCanceledEvent>(result.Message.Value);

                    var existing = db.BookedAppointments
                        .FirstOrDefault(x => x.AppointmentId == evt.AppointmentId);

                    if (existing != null)
                        db.BookedAppointments.Remove(existing);
                }
                db.SaveChanges();
                consumer.Commit(result);
            }
        }
    }
}
