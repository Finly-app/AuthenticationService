using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

public class RoleConfiguration : IEntityTypeConfiguration<Role> {
    public void Configure(EntityTypeBuilder<Role> builder) {
        builder.ToTable("roles");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
               .HasColumnName("id")
               .HasColumnType("uuid");

        builder.Property(r => r.Name)
               .HasColumnName("name")
               .HasMaxLength(50)
               .IsRequired();

        var adminId = Guid.Parse("c0cc1b7f-2705-49b9-b746-75a8dac9861d");
        var userId = Guid.Parse("a191243f-1149-4b19-a66c-96541dc2deff");

        builder.HasData(
            new Role(adminId, "SuperAdmin"),
            new Role(userId, "User")
        );
    }
}
