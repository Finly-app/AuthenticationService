using Authentication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Authentication.Persistance.Configurations {
    public class TokenConfiguration : IEntityTypeConfiguration<Token> {
        public void Configure(EntityTypeBuilder<Token> builder) {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                   .HasColumnName("id")
                   .HasColumnType("int")
                   .ValueGeneratedOnAdd();

            builder.Property(t => t.UserId)
                   .HasColumnName("user_id")
                   .HasColumnType("uniqueidentifier");

            builder.Property(t => t.JWTToken)
                   .HasColumnName("token")
                   .HasMaxLength(50);

            builder.Property(t => t.ExpiresAt)
                   .HasColumnName("expires_at")
                   .HasColumnType("datetime");
        }
    }
}
