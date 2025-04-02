namespace Authentication.Domain.DTOs {
    public class AuthenticationResponse {
        public string Token{get; set;}
        public DateTime ExpiresAt{get; set;}
    }
}
