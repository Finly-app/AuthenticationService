using Authentication.Application.Interfaces;
using Authentication.Application.Security;
using Authentication.Domain.DTOs;
using Authentication.Domain.Entities;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace Authentication.Application.Services {
    public class AuthenticationService : IAuthenticationService {
        private readonly ITokenRepository _tokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly IRoleService _roleService;

        public AuthenticationService(ITokenRepository tokenRepository, IUserRepository userRepository, IConfiguration configuration, IRoleService roleService, IUserService userService) {
            _tokenRepository = tokenRepository;
            _userRepository = userRepository;
            _configuration = configuration;
            _roleService = roleService;
            _userService = userService;
        }

        public LoginResult Login(LoginRequestDto request) {
            // 🔹 Determine whether to use email or username
            string username = request.Username;
            string email = null;

            if (!string.IsNullOrWhiteSpace(username) && username.Contains("@")) {
                email = username;
                username = null;
            }

            User user = _userRepository.GetFullUserWithRolesAndPolicies(username, email);

            if (user == null)
                return new LoginResult { Success = false };

            if (!user.Active)
                return new LoginResult { Success = false, IsInactive = true };

            if (user.TwoFactorEnabled) {
                if (string.IsNullOrWhiteSpace(request.TwoFactorCode)) {
                    return new LoginResult {
                        Success = false,
                        Response = new LoginResponseDto {
                            ErrorMessage = "2FA code required."
                        }
                    };
                }

                var hmacSecret = _configuration["HMAC_SECRET"];

                if (!TotpHelper.VerifyCode(hmacSecret, request.TwoFactorCode)) {
                    return new LoginResult {
                        Success = false,
                        Response = new LoginResponseDto {
                            ErrorMessage = "Invalid 2FA code."
                        }
                    };
                }
            }


            if (!user.EmailConfirmed)
                return new LoginResult { Success = false, EmailNotConfirmed = true };

            bool verified = PasswordHasher.Verify(request.Password, user.Password);

            if (!verified)
                return new LoginResult { Success = false };

            string rawJwtKey = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]).ToString();
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

        public async Task<RegisterResult> RegisterAsync(RegisterRequestDto request) {
            var existing = await _userRepository.FindByEmailOrUsernameAsync(request.Email, request.Username);
            if (existing.EmailExists || existing.UsernameExists) {
                return new RegisterResult {
                    Success = false,
                    ErrorMessage = "Email or Username already exists."
                };
            }

            var hashedPassword = PasswordHasher.Hash(request.Password);
            var newUser = new User(request.Username, hashedPassword, request.Email);
            newUser.Deactivate(); 

            await _userRepository.CreateUserAsync(newUser);

            await _userService.GenerateEmailConfirmationAsync(newUser);

            return new RegisterResult {
                Success = true,
                Response = new RegisterResponseDto {
                    UserId = newUser.Id,
                    Message = "Registration successful. Please confirm your email."
                }
            };
        }

    }
}
