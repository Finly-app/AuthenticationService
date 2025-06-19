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
                if (result.IsInactive) {
                    return StatusCode(StatusCodes.Status403Forbidden, new LoginResponseDto {
                        ErrorMessage = "Account is inactive."
                    });
                }

                if (result.EmailNotConfirmed) {
                    return StatusCode(StatusCodes.Status403Forbidden, new LoginResponseDto {
                        ErrorMessage = "Email is not confirmed."
                    });
                }

                return Unauthorized(new LoginResponseDto {
                    ErrorMessage = result.Response?.ErrorMessage ?? "Invalid username or password."
                });
            }

            return Ok(result.Response);
        }


        [HttpPost("register")]
        public async Task<ActionResult<RegisterResponseDto>> Register([FromBody] RegisterRequestDto request) {
            var result = await _authenticationService.RegisterAsync(request);

            if (!result.Success) {
                return BadRequest(new { error = result.ErrorMessage });
            }

            return Ok(result.Response);
        }

    }
}
