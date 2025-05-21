using Authentication.Application.Interfaces;
using Authentication.Domain.Entities;
using MailKit.Net.Smtp;
using MimeKit;

namespace Authentication.Application.Services {
    public class UserService : IUserService {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository) {
            _userRepository = userRepository;
        }

        public async Task<User> FindByIdAsync(Guid userId) {
            return await _userRepository.FindByIdAsync(userId);
        }

        public async Task GenerateEmailConfirmationAsync(User user) {
            var confirmationLink = $"https://localhost:8000/confirm?userId={user.Id}"; // For now the gateway

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

        public async Task UpdateUserAsync(User user) {
            await _userRepository.UpdateUserAsync(user);
        }
    }
}
