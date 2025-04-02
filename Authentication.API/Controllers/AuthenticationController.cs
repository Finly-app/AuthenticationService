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
        public LoginResponseDto Login([FromBody] LoginRequestDto request) {
            var loginResponse = _authenticationService.Login(request);

            return loginResponse;
        }
    }
}
