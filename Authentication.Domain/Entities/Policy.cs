using Authentication.Domain.Entities;

public class Policy : Auditable {
    public Guid Id { get; private set; }
    public string Name { get; private set; }

    public Policy(Guid id, string name) {
        Id = id;
        Name = name;
    }
}
