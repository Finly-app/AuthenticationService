using Authentication.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole> {
    public void Configure(EntityTypeBuilder<UserRole> builder) {
        builder.ToTable("users_has_roles");

        builder.HasKey(ur => new { ur.UserId, ur.RoleId });

        builder.Property(ur => ur.UserId).HasColumnName("user_id").HasColumnType("uuid");
        builder.Property(ur => ur.RoleId).HasColumnName("role_id").HasColumnType("uuid");
    }
}
