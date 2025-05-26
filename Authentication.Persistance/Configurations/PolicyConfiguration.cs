using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

public class PolicyConfiguration : IEntityTypeConfiguration<Policy> {
    public void Configure(EntityTypeBuilder<Policy> builder) {
        builder.ToTable("policies");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
               .HasColumnName("id")
               .HasColumnType("uuid");

        builder.Property(p => p.Name)
               .HasColumnName("name")
               .HasMaxLength(100);
    }
}
