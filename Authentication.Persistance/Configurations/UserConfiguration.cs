using Authentication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Authentication.Persistance.Configurations {
    public class UserConfiguration : IEntityTypeConfiguration<User> {
        public void Configure(EntityTypeBuilder<User> builder) {
            builder.ToTable("users");

            builder.HasKey(u => u.UserId);

            builder.Property(u => u.UserId)
                   .HasColumnName("user_id")
                   .HasColumnType("uuid");

            builder.HasIndex(u => u.Username)
                   .IsUnique();

            builder.Property(u => u.Username)
                   .HasColumnName("username")
                   .HasMaxLength(50);

            builder.Property(u => u.Password)
                   .HasColumnName("password")
                   .HasMaxLength(120);

            builder.HasIndex(u => u.Email)
                   .IsUnique();

            builder.Property(u => u.Email)
                   .HasColumnName("email")
                   .HasMaxLength(100);

            builder.Property(u => u.CreatedAt)
                   .HasColumnName("created_at")
                   .HasColumnType("timestamp with time zone");

            builder.Property(u => u.UpdatedAt)
                   .HasColumnName("updated_at")
                   .HasColumnType("timestamp with time zone");
        }
    }
}
