using Instrux.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Instrux.Infrastructure.Data.Configurations;

public class SchoolClassConfiguration : IEntityTypeConfiguration<SchoolClass>
{
    public void Configure(EntityTypeBuilder<SchoolClass> builder)
    {
        builder.ToTable("SchoolClasses");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Subject).HasMaxLength(100);
        builder.Property(x => x.Section).HasMaxLength(50);
        builder.Property(x => x.Term).HasMaxLength(50);
        builder.Property(x => x.ColorHex).HasMaxLength(9).HasDefaultValue("#4F46E5");
    }
}
