using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Authentication.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using QRCoder;

namespace Authentication.API.Controllers {
    [ApiController]
    [Route("verification")]
    public class VerificationController : ControllerBase {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ITotpCacheService _cacheService;

        public VerificationController(
            IUserRepository userRepository,
            IConfiguration configuration,
            ITotpCacheService cacheService) {
            _userRepository = userRepository;
            _configuration = configuration;
            _cacheService = cacheService;
        }

        [HttpPost("2fa/setup")]
        public async Task<IActionResult> SetupTwoFactor([FromQuery] Guid userId) {
            var user = await _userRepository.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            if (user.TwoFactorEnabled)
                return BadRequest("2FA is already enabled.");

            string uniqueSecret = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

            await _cacheService.SetTempSecretAsync(user.Id, uniqueSecret, TimeSpan.FromMinutes(10));

            string base32Secret = TotpHelper.Base32Encode(Encoding.UTF8.GetBytes(uniqueSecret));

            string issuer = "Finly";
            string email = user.Email;
            string otpAuthUri = $"otpauth://totp/{issuer}:{email}?secret={base32Secret}&issuer={issuer}&digits=6";

            using var qrGenerator = new QRCodeGenerator();
            using var qrData = qrGenerator.CreateQrCode(otpAuthUri, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new Base64QRCode(qrData);
            string base64Image = qrCode.GetGraphic(20);

            return Ok(new {
                qrCodeImage = $"data:image/png;base64,{base64Image}",
                manualEntryCode = base32Secret
            });
        }

        [HttpPost("2fa/confirm")]
        public async Task<IActionResult> ConfirmTwoFactor([FromQuery] Guid userId, [FromQuery] string code) {
            var user = await _userRepository.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            var cachedSecret = await _cacheService.GetTempSecretAsync(user.Id);
            if (string.IsNullOrWhiteSpace(cachedSecret))
                return BadRequest("No 2FA setup in progress or it has expired.");

            if (!TotpHelper.VerifyCode(cachedSecret, code))
                return BadRequest("Invalid 2FA code.");

            user.EnableTwoFactor(cachedSecret);
            await _userRepository.UpdateUserAsync(user);
            await _cacheService.RemoveTempSecretAsync(user.Id);

            return Ok("2FA has been successfully enabled.");
        }

        [HttpPost("email/confirm")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] Guid userId) {
            var user = await _userRepository.FindByIdAsync(userId);
            if (user == null)
                return NotFound("Invalid user ID");

            if (user.EmailConfirmed)
                return Ok("Email already confirmed.");

            user.ConfirmEmail();
            await _userRepository.UpdateUserAsync(user);

            return Ok("Email confirmed successfully.");
        }
    }
}
