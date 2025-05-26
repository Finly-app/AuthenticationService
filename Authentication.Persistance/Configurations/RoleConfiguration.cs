using Authentication.Domain.Entities;
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
               .HasMaxLength(50);
    }
}
