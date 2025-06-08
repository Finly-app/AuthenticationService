using Authentication.Application.Interfaces;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public class UserUpdatedHandler : IUserUpdatedHandler {
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<UserUpdatedHandler> _logger;

    public UserUpdatedHandler(IServiceScopeFactory scopeFactory, ILogger<UserUpdatedHandler> logger) {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task HandleAsync(UserUpdatedEvent message, string secret, IProducer<Null, string> producer) {
        using var scope = _scopeFactory.CreateScope();
        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var user = await userRepository.FindByIdAsync(message.UserId);

        var response = new UserUpdatedResponse {
            CorrelationId = message.CorrelationId
        };

        if (user == null) {
            response.Status = "error";
            response.ErrorMessage = "User not found";
        } else {
            user.UpdateUsername(message.Username);
            user.UpdateEmail(message.Email);

            await userRepository.UpdateUserAsync(user);

            response.Status = "success";
        }

        var json = JsonConvert.SerializeObject(response);
        var hmacMessage = MessageHelper.CreateHmacMessage(json, secret);
        await producer.ProduceAsync(message.ReplyTo, hmacMessage);

        _logger.LogInformation("Handled user update for ID: {UserId}", message.UserId);
    }
}
