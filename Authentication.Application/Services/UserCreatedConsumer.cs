using Authentication.Application.Interfaces;
using Authentication.Application.Security;
using Authentication.Domain.Entities;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Authentication.Application.Services {
    public class UserCreatedConsumer : BackgroundService {
        private readonly IConsumer<Ignore, string> _consumer;
        private readonly ILogger<UserCreatedConsumer> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public UserCreatedConsumer(ILogger<UserCreatedConsumer> logger, IServiceScopeFactory serviceScopeFactory, IConfiguration configuration) {
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

            var producerConfig = new ProducerConfig {
                BootstrapServers = _configuration["Kafka:BootstrapServers"]
            };
            var producer = new ProducerBuilder<Null, string>(producerConfig).Build();

            return Task.Run(async () => {
                while (!stoppingToken.IsCancellationRequested) {
                    try {
                        var cr = _consumer.Consume(stoppingToken);
                        var message = JsonConvert.DeserializeObject<UserCreatedEvent>(cr.Message.Value);
                        _logger.LogInformation("Received user created event: {@Event}", message);

                        var scope = _serviceScopeFactory.CreateScope();
                        var _userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

                        var existence = await _userRepository.FindByEmailOrUsernameAsync(message.Email, message.Username);

                        if (existence.EmailExists || existence.UsernameExists) {
                            var errorMessages = new List<string>();
                            if (existence.EmailExists) errorMessages.Add("Email already exists");
                            if (existence.UsernameExists) errorMessages.Add("Username already exists");

                            var errorResponse = new UserCreatedResponse {
                                CorrelationId = message.CorrelationId,
                                UserId = Guid.Empty,
                                Status = "error",
                                ErrorMessage = string.Join(" and ", errorMessages)
                            };

                            var errorJson = JsonConvert.SerializeObject(errorResponse);
                            await producer.ProduceAsync(message.ReplyTo, new Message<Null, string> { Value = errorJson });
                            continue;
                        }

                        string hashedPassword = PasswordHasher.Hash(message.Password);
                        var user = new User(Guid.NewGuid(), message.Username, hashedPassword, message.Email);
                        await _userRepository.CreateUserAsync(user);

                        var response = new UserCreatedResponse {
                            CorrelationId = message.CorrelationId,
                            UserId = user.Id,
                            Status = "success"
                        };
                        var responseJson = JsonConvert.SerializeObject(response);
                        await producer.ProduceAsync(message.ReplyTo, new Message<Null, string> { Value = responseJson });

                        _logger.LogInformation("Sent user creation response with CorrelationId {CorrelationId}", message.CorrelationId);
                    } catch (Exception ex) {
                        _logger.LogError(ex, "Error consuming or processing Kafka message.");
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