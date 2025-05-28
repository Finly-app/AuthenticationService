using Authentication.Application.Interfaces;
using Authentication.Application.Security;
using Authentication.Domain.Entities;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public class UserCreatedHandler : IUserCreatedHandler {
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<UserCreatedHandler> _logger;

    public UserCreatedHandler(IServiceScopeFactory scopeFactory, ILogger<UserCreatedHandler> logger) {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task HandleAsync(UserCreatedEvent message, string secret, IProducer<Null, string> producer) {
        using var scope = _scopeFactory.CreateScope();
        var _userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var _userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        var _roleService = scope.ServiceProvider.GetRequiredService<IRoleService>();

        var existence = await _userRepository.FindByEmailOrUsernameAsync(message.Email, message.Username);
        UserCreatedResponse response;

        if (existence.EmailExists || existence.UsernameExists) {
            var errors = new List<string>();
            if (existence.EmailExists) errors.Add("Email already exists");
            if (existence.UsernameExists) errors.Add("Username already exists");

            response = new UserCreatedResponse {
                CorrelationId = message.CorrelationId,
                UserId = Guid.Empty,
                Status = "error",
                ErrorMessage = string.Join(" and ", errors)
            };
        } else {
            var hashedPassword = PasswordHasher.Hash(message.Password);
            var defaultRole = await _roleService.GetByIdAsync(Guid.Parse("a191243f-1149-4b19-a66c-96541dc2deff"));
            if (defaultRole == null)
                throw new Exception("Default role 'User' not found.");

            var user = new User(Guid.NewGuid(), message.Username, hashedPassword, message.Email, defaultRole.Id);
            await _userRepository.CreateUserAsync(user);

            _userService.GenerateEmailConfirmationAsync(user);

            response = new UserCreatedResponse {
                CorrelationId = message.CorrelationId,
                UserId = user.Id,
                Status = "success"
            };
        }

        var json = JsonConvert.SerializeObject(response);
        var hmacMessage = MessageHelper.CreateHmacMessage(json, secret);
        await producer.ProduceAsync(message.ReplyTo, hmacMessage);

        _logger.LogInformation("Handled user creation. CorrelationId: {CorrelationId}", message.CorrelationId);
    }
}
