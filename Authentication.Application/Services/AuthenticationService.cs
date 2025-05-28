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
        private readonly IRoleService _roleService;

        public AuthenticationService(ITokenRepository tokenRepository, IUserRepository userRepository, IConfiguration configuration, IRoleService roleService) {
            _tokenRepository = tokenRepository;
            _userRepository = userRepository;
            _configuration = configuration;
            _roleService = roleService;
        }

        public LoginResult Login(LoginRequestDto request) {
            User user = _userRepository.GetFullUserWithRolesAndPolicies(request.Username, request.Email); // <- you'll need this custom method

            if (user == null)
                return new LoginResult { Success = false };

            if (!user.Active)
                return new LoginResult { Success = false, IsInactive = true };

            if (!user.EmailConfirmed)
                return new LoginResult { Success = false, EmailNotConfirmed = true };

            bool verified = PasswordHasher.Verify(request.Password, user.Password);

            if (!verified)
                return new LoginResult { Success = false };

            string rawJwtKey = _configuration["Jwt:Key"];
            string jwtSecret = _configuration["JWT_SECRET"];
            string jwtKey = rawJwtKey?.Replace("{JWT_SECRET}", jwtSecret ?? string.Empty);

            if (string.IsNullOrWhiteSpace(jwtKey))
                throw new Exception("Internal Server Error");

            var roles = user.Role != null
                 ? new List<string> { user.Role.Name }
                 : new List<string>();

            var rolePolicies = user.RoleId != Guid.Empty
                 ? _roleService.GetAllPoliciesForRoleAndInherited(user.RoleId).Select(p => p.Name)
                 : Enumerable.Empty<string>();

            var userPolicies = user.Policies
                .Select(up => up.Policy?.Name);

            var allPolicies = rolePolicies
                .Concat(userPolicies)
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Distinct()
                .ToList();

            string token = TokenGenerator.GenerateToken(
                user.Id.ToString(),
                user.Username,
                user.Email,
                roles,
                allPolicies,
                jwtKey,
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
