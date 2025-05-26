using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

public class UserPolicyConfiguration : IEntityTypeConfiguration<UserPolicy> {
    public void Configure(EntityTypeBuilder<UserPolicy> builder) {
        builder.ToTable("users_has_policies");

        builder.HasKey(up => new { up.UserId, up.PolicyId });

        builder.Property(up => up.UserId).HasColumnName("user_id").HasColumnType("uuid");
        builder.Property(up => up.PolicyId).HasColumnName("policy_id").HasColumnType("uuid");
    }
}
