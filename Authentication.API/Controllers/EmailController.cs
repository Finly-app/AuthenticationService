using Authentication.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.API.Controllers {
    [ApiController]
    [Route("email")]
    public class EmailController : ControllerBase {
        private readonly IUserRepository _userRepository;

        public EmailController(IUserRepository userRepository) {
            _userRepository = userRepository;
        }

        [HttpPost("confirm")]
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
