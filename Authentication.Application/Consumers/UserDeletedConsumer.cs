using Authentication.Application.Interfaces;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public class UserDeletedConsumer : BackgroundService {
    private readonly ILogger<UserDeletedConsumer> _logger;
    private readonly IConfiguration _config;
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly IProducer<Null, string> _producer;
    private readonly IUserDeletedHandler _handler;

    public UserDeletedConsumer(
        ILogger<UserDeletedConsumer> logger,
        IConfiguration config,
        IUserDeletedHandler handler) {

        _logger = logger;
        _config = config;
        _handler = handler;

        var consumerConfig = new ConsumerConfig {
            BootstrapServers = config["Kafka:BootstrapServers"],
            GroupId = "auth-service-updated-consumer-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        _consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();

        var producerConfig = new ProducerConfig {
            BootstrapServers = config["Kafka:BootstrapServers"]
        };

        _producer = new ProducerBuilder<Null, string>(producerConfig).Build();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken) {
        _consumer.Subscribe("user-deleted-topic");
        var hmacSecret = _config["HMAC_SECRET"];

        return Task.Run(async () => {
            while (!stoppingToken.IsCancellationRequested) {
                try {
                    var result = _consumer.Consume(stoppingToken);

                    if (!MessageHelper.ValidateMessageHmac(result, hmacSecret)) {
                        _logger.LogWarning("Invalid HMAC - ignoring message.");
                        continue;
                    }

                    var userDeleted = JsonConvert.DeserializeObject<UserDeletedEvent>(result.Message.Value);
                    await _handler.HandleAsync(userDeleted, hmacSecret, _producer);
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
