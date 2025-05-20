using Authentication.Domain.DTOs;

public class LoginResult {
    public bool Success { get; set; }
    public bool IsInactive { get; set; }
    public LoginResponseDto Response { get; set; }
}
