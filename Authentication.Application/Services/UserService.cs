using Authentication.Application.Interfaces;
using Authentication.Mapping;
using MailKit.Net.Smtp;
using MimeKit;

namespace Authentication.Application.Services {
    public class UserService : IUserService {
        private readonly IUserRepository _userRepository;
        private readonly IRoleService _roleService;

        public UserService(IUserRepository userRepository, IRoleService roleService) {
            _userRepository = userRepository;
            _roleService = roleService;
        }

        public async Task<bool> AssignRoleToUserAsync(Guid userId, Guid roleId) {
            return await _userRepository.AssignRoleToUserAsync(userId, roleId);
        }

        public async Task<bool> AssignUserPoliciesAsync(Guid userId, List<Guid> policyIds) {
            return await _userRepository.AssignUserPoliciesAsync(userId, policyIds);
        }

        public async Task<User> FindByIdAsync(Guid userId) {
            return await _userRepository.FindByIdAsync(userId);
        }

        public async Task GenerateEmailConfirmationAsync(User user) {
            var confirmationLink = $"http://localhost:8000/confirm?userId={user.Id}"; // For now the gateway

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Finly", "no-reply@finly.com"));
            message.To.Add(new MailboxAddress(user.Username, user.Email));
            message.Subject = "Confirm your email";

            message.Body = new TextPart("html") {
                Text = $"<p>Click the link to confirm your email:</p><a href='{confirmationLink}'>{confirmationLink}</a>"
            };

            // Use Ethereal SMTP server for now (dev-only)
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.ethereal.email", 587, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync("charlotte.medhurst@ethereal.email", "GYC6cJvAtETazPCyC2"); 
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

        public async Task<bool> UpdateUserRoleAsync(Guid userId, Guid roleId) {
            return await _userRepository.UpdateUserRoleAsync(userId, roleId);
        }
    }
}
