using Authentication.Domain.Entities;

public class Role : Auditable {
    public Guid Id { get; private set; }
    public string Name { get; private set; }

    public ICollection<UserRole> Users { get; private set; } = new List<UserRole>();

    public ICollection<RolePolicy> Policies { get; private set; } = new List<RolePolicy>();

    public Role(Guid id, string name) {
        Id = id;
        Name = name;
    }
}
