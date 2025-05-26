using Authentication.Domain.Entities;

public class User : Auditable {
    public Guid Id { get; private set; }
    public string Username { get; private set; }
    public string Password { get; private set; }
    public string Email { get; private set; }
    public bool EmailConfirmed { get; private set; }
    public bool Active { get; private set; }
    public DateTime? DeactivatedAt { get; private set; }

    public ICollection<UserRole> Roles { get; private set; } = new List<UserRole>();
    public ICollection<UserPolicy> Policies { get; private set; } = new List<UserPolicy>();

    public User(Guid id, string username, string password, string email) {
        Id = id;
        Username = username;
        Password = password;
        Email = email;
        Active = true;
        EmailConfirmed = false;
    }

    public void ConfirmEmail() {
        if (!EmailConfirmed) {
            EmailConfirmed = true;
        }
    }
}
