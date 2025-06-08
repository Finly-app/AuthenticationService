using Authentication.Application.Interfaces;
using Authentication.Domain.Entities;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public class UserDeletedHandler : IUserDeletedHandler {
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<UserDeletedHandler> _logger;

    public UserDeletedHandler(IServiceScopeFactory scopeFactory, ILogger<UserDeletedHandler> logger) {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task HandleAsync(UserDeletedEvent message, string secret, IProducer<Null, string> producer) {
        using var scope = _scopeFactory.CreateScope();
        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

        var user = await userRepository.FindByIdAsync(message.UserId);

        var response = new UserDeletedResponse {
            CorrelationId = message.CorrelationId
        };

        if (user == null) {
            response.Status = "error";
            response.ErrorMessage = "User not found";
        } else {
            user.Delete(); // Mark Deleted = true
            await userRepository.UpdateUserAsync(user); // Save it

            response.Status = "success";
        }

        var json = JsonConvert.SerializeObject(response);
        var hmacMessage = MessageHelper.CreateHmacMessage(json, secret);
        await producer.ProduceAsync(message.ReplyTo, hmacMessage);

        _logger.LogInformation("Soft-deleted user with ID {UserId}", message.UserId);
    }
}
