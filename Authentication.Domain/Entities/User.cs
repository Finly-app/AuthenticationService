namespace Authentication.Domain.Entities {
    public class User : Auditable {
        public Guid Id { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public string Email { get; private set; }
        public bool Active { get; private set; }
        public DateTime? DeactivatedAt { get; private set; }

        public User(Guid Id, string username, string password, string email) { 
            Id = Id;
            Username = username;
            Password = password;
            Email = email;
            Active = true;
        }
    }
}
