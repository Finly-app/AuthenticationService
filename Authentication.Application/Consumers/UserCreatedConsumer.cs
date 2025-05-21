using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public class UserCreatedConsumer : BackgroundService {
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly IProducer<Null, string> _producer;
    private readonly IConfiguration _config;
    private readonly ILogger<UserCreatedConsumer> _logger;
    private readonly IUserCreatedHandler _handler;

    public UserCreatedConsumer(
        ILogger<UserCreatedConsumer> logger,
        IConfiguration configuration,
        IUserCreatedHandler handler) {

        _logger = logger;
        _config = configuration;
        _handler = handler;

        var consumerConfig = new ConsumerConfig {
            GroupId = "auth-service-consumer-group",
            BootstrapServers = _config["Kafka:BootstrapServers"],
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();

        var producerConfig = new ProducerConfig {
            BootstrapServers = _config["Kafka:BootstrapServers"]
        };

        _producer = new ProducerBuilder<Null, string>(producerConfig).Build();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken) {
        _consumer.Subscribe("user-created-topic");
        var hmacSecret = _config["HMAC_SECRET"];

        return Task.Run(async () => {
            while (!stoppingToken.IsCancellationRequested) {
                try {
                    var result = _consumer.Consume(stoppingToken);

                    if (!MessageHelper.ValidateMessageHmac(result, hmacSecret)) {
                        _logger.LogWarning("Invalid HMAC - ignoring message.");
                        continue;
                    }

                    var userCreated = JsonConvert.DeserializeObject<UserCreatedEvent>(result.Message.Value);
                    await _handler.HandleAsync(userCreated, hmacSecret, _producer);
                } catch (Exception ex) {
                    _logger.LogError(ex, "Error processing Kafka message.");
                }
            }
        }, stoppingToken);
    }

    public override void Dispose() {
        _consumer.Close();
        _consumer.Dispose();
        _producer.Dispose();
        base.Dispose();
    }
}
