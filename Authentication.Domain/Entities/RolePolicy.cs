using Authentication.Domain.Entities;

public class RolePolicy : Auditable {
    public Guid RoleId { get; private set; }
    public Guid PolicyId { get; private set; }

    public Policy Policy { get; private set; }

    public RolePolicy(Guid roleId, Guid policyId) {
        RoleId = roleId;
        PolicyId = policyId;
    }
}
