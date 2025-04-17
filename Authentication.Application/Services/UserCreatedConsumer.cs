using Authentication.Application.Interfaces;
using Authentication.Application.Security;
using Authentication.Domain.Entities;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shared.Contracts.Events;

namespace Authentication.Application.Services {
    public class UserCreatedConsumer : BackgroundService {
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly ILogger<UserCreatedConsumer> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public UserCreatedConsumer(ILogger<UserCreatedConsumer> logger, IServiceScopeFactory serviceScopeFactory,  IConfiguration configuration) {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _configuration = configuration;

            var config = new ConsumerConfig {
                GroupId = "auth-service-consumer-group",
                BootstrapServers = _configuration["Kafka:BootstrapServers"],
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken) {
            _consumer.Subscribe("user-created-topic");

            return Task.Run(() => {
                while (!stoppingToken.IsCancellationRequested) {
                    try {
                        var cr = _consumer.Consume(stoppingToken);

                        var message = JsonConvert.DeserializeObject<UserCreatedEvent>(cr.Message.Value);
                        _logger.LogInformation("Received user created event: {@Event}", message);

                        var scope = _serviceScopeFactory.CreateScope();
                        var _userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

                        string hashedPassword = PasswordHasher.Hash(message.Password);

                        User user = new User(message.Id, message.Username, hashedPassword, message.Email);

                        _userRepository.CreateUserAsync(user);
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