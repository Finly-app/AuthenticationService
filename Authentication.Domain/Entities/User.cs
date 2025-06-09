using Authentication.Domain.Entities;

public class User : Auditable {
    public Guid Id { get; private set; }
    public string Username { get; private set; }
    public string Password { get; private set; }
    public string Email { get; private set; }
    public bool EmailConfirmed { get; private set; }
    public bool Active { get; private set; }
    public DateTime? DeactivatedAt { get; private set; }
    public bool Deleted { get; private set; }
    public Guid RoleId { get; private set; }
    public Role Role { get; private set; }

    public ICollection<UserPolicy> Policies { get; private set; } = new List<UserPolicy>();

    public User(Guid id, string username, string password, string email, Guid roleId) {
        Id = id;
        Username = username;
        Password = password;
        Email = email;
        RoleId = roleId;
        Active = true;
        EmailConfirmed = false;
        Deleted = false;
    }

    public void ConfirmEmail() {
        if (!EmailConfirmed) {
            EmailConfirmed = true;
        }
    }

    public void AssignRole(Guid roleId) {
        RoleId = roleId;
    }

    public void UpdateUsername(string username) {
        if (!string.IsNullOrWhiteSpace(username) && username != Username) {
            Username = username;
        }
    }

    public void UpdateEmail(string email) {
        if (!string.IsNullOrWhiteSpace(email) && email != Email) {
            Email = email;
        }
    }

    public void Delete() {
        Deleted = true;
    }

    public void Deactivate() {
        Active = false;
        DeactivatedAt = DateTime.UtcNow;
    }

    public void Activate() {
        if (!Active) {
            Active = true;
            DeactivatedAt = null;
        }
    }
}
