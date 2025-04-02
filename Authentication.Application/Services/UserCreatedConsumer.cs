using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shared.Contracts.Events;

namespace Authentication.Application.Services {
    public class UserCreatedConsumer : BackgroundService {
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly ILogger<UserCreatedConsumer> _logger;

        public UserCreatedConsumer(ILogger<UserCreatedConsumer> logger) {
            var config = new ConsumerConfig {
                GroupId = "auth-service-consumer-group",
                BootstrapServers = "localhost:9092",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken) {
            _consumer.Subscribe("user-created-topic");

            return Task.Run(() => {
                while (!stoppingToken.IsCancellationRequested) {
                    try {
                        var cr = _consumer.Consume(stoppingToken);

                        var message = JsonConvert.DeserializeObject<UserCreatedEvent>(cr.Message.Value);
                        _logger.LogInformation("Received user created event: {@Event}", message);

                        // TODO: Handle event here (e.g., create a token or store something)
                    } catch (Exception ex) {
                        _logger.LogError(ex, "Error consuming Kafka message.");
                    }
                }
            }, stoppingToken);
        }

        public override void Dispose() {
            _consumer.Close();
            _consumer.Dispose();
            base.Dispose();
        }
    }
}