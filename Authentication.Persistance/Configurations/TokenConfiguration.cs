using Authentication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Authentication.Persistance.Configurations {
    public class TokenConfiguration : IEntityTypeConfiguration<Token> {
        public void Configure(EntityTypeBuilder<Token> builder) {
            builder.ToTable("tokens");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                   .HasColumnName("id")
                   .UseIdentityAlwaysColumn();

            builder.Property(t => t.UserId)
                   .HasColumnName("user_id")
                   .HasColumnType("uuid");

            builder.Property(t => t.JwtToken)
                   .HasColumnName("token")
                   .HasColumnType("text");

            builder.Property(t => t.ExpiresAt)
                   .HasColumnName("expires_at")
                   .HasColumnType("timestamp");

            builder.Property(t => t.CreatedAt)
                   .HasColumnName("created_at")
                   .HasColumnType("timestamp");

            builder.Property(t => t.UpdatedAt)
                   .HasColumnName("updated_at")
                   .HasColumnType("timestamp");
        }
    }
}
