using Authentication.Application.Interfaces;
using Authentication.Application.Security;
using Authentication.Domain.DTOs;
using Authentication.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace Authentication.Application.Services {
    public class AuthenticationService : IAuthenticationService {
        private readonly ITokenRepository _tokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthenticationService(ITokenRepository tokenRepository, IUserRepository userRepository, IConfiguration configuration) {
            _tokenRepository = tokenRepository;
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public LoginResult Login(LoginRequestDto request) {
            User user = _userRepository.GetHashedPassword(request.Username, request.Email);

            if (user == null)
                return new LoginResult { Success = false };

            if (!user.Active)
                return new LoginResult { Success = false, IsInactive = true };

            bool verified = PasswordHasher.Verify(request.Password, user.Password);

            if (!verified)
                return new LoginResult { Success = false };

            string token = TokenGenerator.GenerateToken(
                user.Id.ToString(),
                user.Username,
                user.Email,
                _configuration["Jwt:Key"],
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"]
            );

            var expiresAt = DateTime.UtcNow.AddHours(1);

            _tokenRepository.StoreToken(new Token(user.Id, token, expiresAt));

            return new LoginResult {
                Success = true,
                Response = new LoginResponseDto {
                    Token = token,
                    ExpiresAt = expiresAt
                }
            };
        }

    }
}
