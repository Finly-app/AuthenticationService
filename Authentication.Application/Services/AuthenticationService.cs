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

        public LoginResponseDto Login(LoginRequestDto request) {
            User user = _userRepository.GetHashedPassword(request.Username, request.Email);

            bool verified = PasswordHasher.Verify(request.Password, user.Password);

            if (!verified) return null;

            string token = TokenGenerator.GenerateToken(
                user.UserId.ToString(),
                user.Username,
                user.Email,
                _configuration["Jwt:Key"],
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"]
            );

            var expiresAt = DateTime.UtcNow.AddHours(1);

            Token tokenEntity = new Token(user.UserId, token, expiresAt);

            _tokenRepository.StoreToken(tokenEntity);

            return new LoginResponseDto {
                Token = token,
                ExpiresAt = expiresAt
            };
        }
    }
}
