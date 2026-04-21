using Confluent.Kafka;
using System.Text.Json;

namespace HCMS.AppointmentService.Infrastructure.Kafka
{
    public class KafkaProducer
    {
        private readonly IProducer<string, string> _producer;

        public KafkaProducer(IConfiguration config)
        {
            var producerConfig = new ProducerConfig
            {
                BootstrapServers = config["Kafka:BootstrapServers"]
            };

            _producer = new ProducerBuilder<string, string>(producerConfig).Build();
        }

        public async Task ProduceAsync(string topic, object message)
        {
            var json = JsonSerializer.Serialize(message);

            await _producer.ProduceAsync(topic, new Message<string, string>
            {
                Key = Guid.NewGuid().ToString(),
                Value = json
            });
        }
    }
}
