using Authentication.Domain.Entities;

public class UserRole : Auditable {
    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }

    public Role Role { get; private set; }

    public UserRole(Guid userId, Guid roleId) {
        UserId = userId;
        RoleId = roleId;
    }
}
