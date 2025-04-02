namespace Authentication.Domain.Entities {
    public class Token : Auditable {
        public int Id { get; private set; }
        public Guid UserId { get; private set; }
        public string JwtToken { get; private set; }
        public DateTime ExpiresAt { get; private set; }

        public Token(Guid userId, string jwtToken, DateTime expiresAt) {
            UserId = userId;
            JwtToken = jwtToken;
            ExpiresAt = expiresAt;
        }
    }
}
