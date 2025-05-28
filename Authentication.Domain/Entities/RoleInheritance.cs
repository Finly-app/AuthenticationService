public class RoleInheritance {
    public Guid ParentRoleId { get; private set; }
    public Guid ChildRoleId { get; private set; }

    public Role ParentRole { get; private set; }
    public Role ChildRole { get; private set; }

    public RoleInheritance(Guid parentRoleId, Guid childRoleId) {
        ParentRoleId = parentRoleId;
        ChildRoleId = childRoleId;
    }
}
