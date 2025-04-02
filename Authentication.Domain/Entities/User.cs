namespace Authentication.Domain.Entities {
    public class User : Auditable {
        public Guid UserId { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string Email { get; private set; }

        public User(Guid userId, string username, string password, string email) { 
            UserId = userId;
            Username = username;
            Password = password;
            Email = email;
        }
    }
}
