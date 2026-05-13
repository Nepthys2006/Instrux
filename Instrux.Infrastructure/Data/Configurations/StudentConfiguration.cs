using Instrux.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Instrux.Infrastructure.Data.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Students");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.StudentIdentifier).HasMaxLength(50);
        builder.Property(x => x.Email).HasMaxLength(200);
        builder.HasIndex(x => x.StudentIdentifier).IsUnique().HasFilter("[StudentIdentifier] IS NOT NULL");

        builder.HasOne(x => x.Class)
            .WithMany(x => x.Students)
            .HasForeignKey(x => x.ClassId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
