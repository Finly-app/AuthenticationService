using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class RoleInheritanceConfiguration : IEntityTypeConfiguration<RoleInheritance> {
    public void Configure(EntityTypeBuilder<RoleInheritance> builder) {
        builder.ToTable("role_inheritance");

        builder.HasKey(ri => new { ri.ChildRoleId, ri.ParentRoleId });

        builder.HasOne(ri => ri.ChildRole)
            .WithMany()
            .HasForeignKey(ri => ri.ChildRoleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ri => ri.ParentRole)
            .WithMany()
            .HasForeignKey(ri => ri.ParentRoleId)
            .OnDelete(DeleteBehavior.Restrict);

        var superAdminId = Guid.Parse("c0cc1b7f-2705-49b9-b746-75a8dac9861d");
        var adminId = Guid.Parse("33ee7453-b06c-4959-9377-badf265ee52d");
        var userId = Guid.Parse("a191243f-1149-4b19-a66c-96541dc2deff");

        builder.HasData(
            new RoleInheritance(superAdminId, adminId),
            new RoleInheritance(adminId, userId)
        );
    }
}
