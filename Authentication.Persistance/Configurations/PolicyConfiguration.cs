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
            new Policy(Guid.Parse("3fbed1fd-75e7-42a9-b017-8a2be84c81f1"), "users:read"),
            new Policy(Guid.Parse("9a308869-5e11-4480-916b-cef3908797dc"), "users:create"),
            new Policy(Guid.Parse("cdd60f4c-521f-4a45-8b87-1b84b69d49c1"), "users:update"),
            new Policy(Guid.Parse("321aa912-e34e-4f41-9dbf-f5f61a3951f2"), "users:delete")
        );
    }
}
