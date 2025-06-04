using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public class UserAuthFilterConsumer : BackgroundService {
    private readonly IConfiguration _config;
    private readonly ILogger<UserAuthFilterConsumer> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly IProducer<Null, string> _producer;

    public UserAuthFilterConsumer(
        IConfiguration config,
        ILogger<UserAuthFilterConsumer> logger,
        IServiceScopeFactory scopeFactory) {

        _config = config;
        _logger = logger;
        _scopeFactory = scopeFactory;

        _consumer = new ConsumerBuilder<Ignore, string>(new ConsumerConfig {
            BootstrapServers = config["Kafka:BootstrapServers"],
            GroupId = "auth-userfilter-consumer",
            AutoOffsetReset = AutoOffsetReset.Earliest
        }).Build();

        _producer = new ProducerBuilder<Null, string>(new ProducerConfig {
            BootstrapServers = config["Kafka:BootstrapServers"]
        }).Build();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken) {
        _consumer.Subscribe("user-auth-filter-request-topic");
        var hmac = _config["HMAC_SECRET"];

        return Task.Run(async () => {
            while (!stoppingToken.IsCancellationRequested) {
                try {
                    var cr = _consumer.Consume(stoppingToken);
                    if (!MessageHelper.ValidateMessageHmac(cr, hmac)) continue;

                    var request = JsonConvert.DeserializeObject<UserAuthFilterRequest>(cr.Message.Value);
                    if (request == null) continue;

                    using var scope = _scopeFactory.CreateScope();
                    var repo = scope.ServiceProvider.GetRequiredService<IUserRepository>();

                    List<User> users = request.Filter switch {
                        "active" => await repo.GetActiveUsersAsync(),
                        "deactive" => await repo.GetDeactivatedUsersAsync(),
                        _ => await repo.GetAllUsersAsync()
                    };

                    var response = new UserAuthInfoResponse {
                        CorrelationId = request.CorrelationId,
                        Users = users.Select(u => new UserAuthInfo {
                            Id = u.Id,
                            Username = u.Username,
                            Email = u.Email,
                            EmailConfirmed = u.EmailConfirmed,
                            Active = u.Active
                        }).ToList()
                    };

                    var json = JsonConvert.SerializeObject(response);
                    var msg = MessageHelper.CreateHmacMessage(json, hmac);

                    await _producer.ProduceAsync(request.ReplyTo, msg);
                } catch (Exception ex) {
                    _logger.LogError(ex, "UserAuthFilterConsumer error");
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
