using Authentication.Application.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using MimeKit;
using System.Security.Claims;


namespace Authentication.Application.Services {
    public class UserService : IUserService {
        private readonly IUserRepository _userRepository;
        private readonly IRoleService _roleService;
        private readonly IHttpContextAccessor _contextAccessor;

        public UserService(IUserRepository userRepository, IRoleService roleService, IHttpContextAccessor contextAccessor) {
            _userRepository = userRepository;
            _roleService = roleService;
            _contextAccessor = contextAccessor;
        }


        private async Task<bool> CurrentUserIsSuperAdminAsync() {
            var user = _contextAccessor.HttpContext?.User;
            var userIdClaim = user?.FindFirst("sub")?.Value ?? user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
                return false;

            if (!Guid.TryParse(userIdClaim, out var userId))
                return false;

            var role = await GetUserRoleAsync(userId);
            return role?.Name?.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase) == true;
        }


        public async Task<bool> AssignRoleToUserAsync(Guid userId, Guid roleId) {
            var role = await _roleService.GetByIdAsync(roleId);
            if (role == null)
                return false;

            if (role.Name == "SuperAdmin") {
                var alreadyExists = await _userRepository.SuperAdminExistsAsync();
                if (alreadyExists)
                    throw new InvalidOperationException("Only one SuperAdmin is allowed in the system.");
            }

            if (role.Name == "Admin") {
                var isSuperAdmin = await CurrentUserIsSuperAdminAsync();
                if (!isSuperAdmin)
                    throw new UnauthorizedAccessException("Only SuperAdmins can assign Admin roles.");
            }

            return await _userRepository.AssignRoleToUserAsync(userId, roleId);
        }


        public async Task<bool> AssignUserPoliciesAsync(Guid userId, List<Guid> policyIds) {
            return await _userRepository.AssignUserPoliciesAsync(userId, policyIds);
        }

        public async Task<User> FindByIdAsync(Guid userId) {
            return await _userRepository.FindByIdAsync(userId);
        }

        public async Task GenerateEmailConfirmationAsync(User user) {
            var confirmationLink = $"http://localhost:8000/confirm?userId={user.Id}"; // Replace with actual domain if needed

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Finly", "no-reply@finly.com"));
            message.To.Add(new MailboxAddress(user.Username, user.Email));
            message.Subject = "Confirm your email";

            message.Body = new TextPart("html") {
                Text = $"<p>Click the link to confirm your email:</p><a href='{confirmationLink}'>{confirmationLink}</a>"
            };

            using var smtp = new SmtpClient();
            // Connect to MailHog (host: mailhog, port: 1025)
            await smtp.ConnectAsync("localhost", 1025, MailKit.Security.SecureSocketOptions.None);
            // No authentication required for MailHog
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }

        public async Task<List<PolicyDto>> GetUserPoliciesAsync(Guid userId) {
            var user = await _userRepository.GetUserWithPoliciesAndRoleAsync(userId);
            if (user == null) return new();

            // Direct user policies
            var userPolicies = user.Policies
                .Select(up => up.Policy)
                .Where(p => !string.IsNullOrWhiteSpace(p.Name))
                .ToList();

            // Role & inherited policies
            var rolePolicies = new List<Policy>();
            if (user.RoleId != Guid.Empty)
                rolePolicies = await _roleService.GetAllPoliciesForRoleAndInheritedAsync(user.RoleId);

            var allPolicies = userPolicies
                .Concat(rolePolicies)
                .DistinctBy(p => p.Id)
                .ToList();

            return allPolicies.Select(p => new PolicyDto {
                Id = p.Id,
                Name = p.Name
            }).ToList();
        }

        public async Task<RoleDto?> GetUserRoleAsync(Guid userId) {
            var userRole = await _userRepository.GetUserRoleAsync(userId);
            return userRole;
        }

        public async Task<bool> RemoveUserPolicyAsync(Guid userId, Guid policyId) {
            return await _userRepository.RemoveUserPolicyAsync(userId, policyId);
        }

        public async Task UpdateUserAsync(User user) {
            await _userRepository.UpdateUserAsync(user);
        }
    }
}
