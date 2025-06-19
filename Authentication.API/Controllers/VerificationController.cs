using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using Authentication.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using QRCoder;

namespace Authentication.API.Controllers {
    [ApiController]
    [Route("verification")]
    public class VerificationController : ControllerBase {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public VerificationController(IUserService userService, IConfiguration configuration) {
            _userService = userService;
            _configuration = configuration;
        }

        [HttpPost("2fa/setup")]
        public async Task<IActionResult> SetupTwoFactor([FromQuery] Guid userId) {
            var user = await _userService.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            if (user.TwoFactorEnabled)
                return BadRequest("2FA is already enabled.");

            var hmacSecret = _configuration["HMAC_SECRET"];

            string issuer = "Finly";
            string email = user.Email;
            string base32Secret = TotpHelper.Base32Encode(Encoding.UTF8.GetBytes(hmacSecret));
            string otpAuthUri = $"otpauth://totp/{issuer}:{email}?secret={base32Secret}&issuer={issuer}&digits=6";

            using var qrGenerator = new QRCodeGenerator();
            using var qrData = qrGenerator.CreateQrCode(otpAuthUri, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new Base64QRCode(qrData);
            string base64Image = qrCode.GetGraphic(20); 

            user.EnableTwoFactor(hmacSecret);
            await _userService.UpdateUserAsync(user);

            return Ok(new {
                qrCodeImage = $"data:image/png;base64,{base64Image}",
                manualEntryCode = base32Secret
            });
        }


        [HttpPost("email/confirm")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] Guid userId) {
            var user = await _userService.FindByIdAsync(userId);
            if (user == null)
                return NotFound("Invalid user ID");

            if (user.EmailConfirmed)
                return Ok("Email already confirmed.");

            user.ConfirmEmail();
            await _userService.UpdateUserAsync(user);

            return Ok("Email confirmed successfully.");
        }
    }
}
