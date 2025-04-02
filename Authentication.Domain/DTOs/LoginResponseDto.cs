namespace Authentication.Domain.DTOs {
    public class LoginResponseDto {
        public string Token{get; set;}
        public DateTime ExpiresAt{get; set;}
    }
}
