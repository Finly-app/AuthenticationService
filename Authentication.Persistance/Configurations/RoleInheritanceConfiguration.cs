using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

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
    }
}
