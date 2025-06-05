using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

public class PolicyConfiguration : IEntityTypeConfiguration<Policy> {
    public void Configure(EntityTypeBuilder<Policy> builder) {
        builder.ToTable("policies");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
               .HasColumnName("id")
               .HasColumnType("uuid");

        builder.Property(p => p.Name)
               .HasColumnName("name")
               .HasMaxLength(100);

        // Seed initial policies
        builder.HasData(
            // Users
            new Policy(Guid.Parse("3fbed1fd-75e7-42a9-b017-8a2be84c81f1"), "users:read"),
            new Policy(Guid.Parse("9a308869-5e11-4480-916b-cef3908797dc"), "users:create"),
            new Policy(Guid.Parse("cdd60f4c-521f-4a45-8b87-1b84b69d49c1"), "users:update"),
            new Policy(Guid.Parse("321aa912-e34e-4f41-9dbf-f5f61a3951f2"), "users:delete"),

            // Roles
            new Policy(Guid.Parse("a10b5c92-3e9b-4d8a-bf01-19d3f9b6f111"), "roles:read"),
            new Policy(Guid.Parse("b21e5d93-4f7c-4d2e-a401-28a5f0c7e222"), "roles:create"),
            new Policy(Guid.Parse("c32f6e94-5a8d-4d3f-b502-37c7f1d8f333"), "roles:update"),
            new Policy(Guid.Parse("d43f7f95-6b9e-4e40-c603-46e8f2e9f444"), "roles:delete"),
            new Policy(Guid.Parse("e54f8096-7caf-4f51-d704-55f9f3faf555"), "roles:policies:read"),
            new Policy(Guid.Parse("f65f9197-8db0-4f62-e805-64faf4fbf666"), "roles:policies:assign"),
            new Policy(Guid.Parse("067fa298-9ec1-4f73-f906-73fb05fc0777"), "roles:policies:remove"),
            new Policy(Guid.Parse("178fb399-afd2-5074-0a07-82fc16fd1888"), "roles:inheritance:create"),

            // User Roles & Policies
            new Policy(Guid.Parse("289fc49a-c0e3-6185-1b18-91fd27fe2999"), "users:roles:read"),
            new Policy(Guid.Parse("39afd59b-d1f4-7296-2c29-a20e380f3aaa"), "users:roles:assign"),
            new Policy(Guid.Parse("4ab0e69c-e205-83a7-3d3a-b31f491045bb"), "users:policies:read"),
            new Policy(Guid.Parse("5bc1f79d-f316-94b8-4e4b-c42f5a2156cc"), "users:policies:assign"),
            new Policy(Guid.Parse("6cd2089e-0427-a5c9-5f5c-d53f6b3267dd"), "users:policies:remove"),

            // Policies Management
            new Policy(Guid.Parse("7de3199f-1538-b6da-607d-e64f7c4378ee"), "policies:read"),
            new Policy(Guid.Parse("8ef42aa0-2649-c7eb-718e-f75f8d5489ff"), "policies:create"),
            new Policy(Guid.Parse("9ff53bb1-375a-d8fc-829f-08609e659a00"), "policies:update"),
            new Policy(Guid.Parse("a1064cc2-486b-e90d-93a0-1971af76ab11"), "policies:delete")
        );
    }
}
