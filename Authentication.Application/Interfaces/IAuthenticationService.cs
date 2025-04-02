using Authentication.Domain.DTOs;

namespace Authentication.Application.Interfaces {
    public interface IAuthenticationService {
        Task<AuthenticationResponse> GenerateToken(LoginRequest request);
    }
}
