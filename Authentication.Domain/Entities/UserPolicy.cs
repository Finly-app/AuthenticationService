using Authentication.Domain.Entities;

public class UserPolicy : Auditable {
    public Guid UserId { get; private set; }
    public Guid PolicyId { get; private set; }

    public Policy Policy { get; private set; }

    public UserPolicy(Guid userId, Guid policyId) {
        UserId = userId;
        PolicyId = policyId;
    }
}
