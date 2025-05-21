using Authentication.Application.Interfaces;
using Authentication.Domain.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.API.Controllers {
    [Route("authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService) {
            _authenticationService = authenticationService;
        }

        [HttpPost("login")]
        public ActionResult<LoginResponseDto> Login([FromBody] LoginRequestDto request) {
            var result = _authenticationService.Login(request);

            if (!result.Success) {
                if (result.IsInactive)
                    return Forbid();
                else if (result.EmailNotConfirmed)
                    return Forbid();

                    return NotFound(); 
            }

            return Ok(result.Response);
        }

    }
}
