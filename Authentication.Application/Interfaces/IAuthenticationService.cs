using Authentication.Domain.DTOs;

namespace Authentication.Application.Interfaces {
    public interface IAuthenticationService {
        LoginResponseDto Login(LoginRequestDto request);
    }
}
