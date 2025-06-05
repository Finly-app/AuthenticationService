using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public class UserAuthSingleConsumer : BackgroundService {
    private readonly IConfiguration _config;
    private readonly ILogger<UserAuthSingleConsumer> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly IProducer<Null, string> _producer;

    public UserAuthSingleConsumer(
        IConfiguration config,
        ILogger<UserAuthSingleConsumer> logger,
        IServiceScopeFactory scopeFactory) {

        _config = config;
        _logger = logger;
        _scopeFactory = scopeFactory;

        _consumer = new ConsumerBuilder<Ignore, string>(new ConsumerConfig {
            BootstrapServers = config["Kafka:BootstrapServers"],
            GroupId = "auth-user-single-consumer",
            AutoOffsetReset = AutoOffsetReset.Earliest
        }).Build();

        _producer = new ProducerBuilder<Null, string>(new ProducerConfig {
            BootstrapServers = config["Kafka:BootstrapServers"]
        }).Build();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken) {
        _consumer.Subscribe("user-auth-single-request-topic");
        var hmac = _config["HMAC_SECRET"];

        return Task.Run(async () => {
            while (!stoppingToken.IsCancellationRequested) {
                try {
                    var cr = _consumer.Consume(stoppingToken);
                    if (!MessageHelper.ValidateMessageHmac(cr, hmac)) continue;

                    var request = JsonConvert.DeserializeObject<UserAuthSingleRequest>(cr.Message.Value);
                    if (request == null) continue;

                    using var scope = _scopeFactory.CreateScope();
                    var repo = scope.ServiceProvider.GetRequiredService<IUserRepository>();

                    var user = await repo.FindByIdAsync(request.UserId);

                    var response = new UserAuthInfoResponse {
                        CorrelationId = request.CorrelationId,
                        Users = user == null ? new List<UserAuthInfo>() : new List<UserAuthInfo> {
                            new UserAuthInfo {
                                Id = user.Id,
                                Username = user.Username,
                                Email = user.Email,
                                EmailConfirmed = user.EmailConfirmed,
                                Active = user.Active
                            }
                        }
                    };

                    var json = JsonConvert.SerializeObject(response);
                    var msg = MessageHelper.CreateHmacMessage(json, hmac);
                    await _producer.ProduceAsync(request.ReplyTo, msg);
                } catch (Exception ex) {
                    _logger.LogError(ex, "UserAuthSingleConsumer error");
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
