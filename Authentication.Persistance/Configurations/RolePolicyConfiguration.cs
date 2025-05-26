using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

public class RolePolicyConfiguration : IEntityTypeConfiguration<RolePolicy> {
    public void Configure(EntityTypeBuilder<RolePolicy> builder) {
        builder.ToTable("roles_has_policies");

        builder.HasKey(rp => new { rp.RoleId, rp.PolicyId });

        builder.Property(rp => rp.RoleId).HasColumnName("role_id").HasColumnType("uuid");
        builder.Property(rp => rp.PolicyId).HasColumnName("policy_id").HasColumnType("uuid");
    }
}
