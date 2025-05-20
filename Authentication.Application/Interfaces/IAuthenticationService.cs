using Authentication.Domain.DTOs;

namespace Authentication.Application.Interfaces {
    public interface IAuthenticationService {
        LoginResult Login(LoginRequestDto request);
    }
}
