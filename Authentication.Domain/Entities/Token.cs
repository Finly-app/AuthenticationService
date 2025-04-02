namespace Authentication.Domain.Entities {
    public class Token : Auditable {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string JWTToken { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
