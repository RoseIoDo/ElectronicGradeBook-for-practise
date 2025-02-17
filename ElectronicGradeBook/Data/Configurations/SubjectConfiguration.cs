using ElectronicGradeBook.Models.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectronicGradeBook.Data.Configurations
{
    public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
    {
        public void Configure(EntityTypeBuilder<Subject> builder)
        {
            builder.ToTable("Subjects");
            builder.HasKey(s => s.Id);

            builder.Property(s => s.FullName)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(s => s.ShortName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(s => s.Code)
                .HasMaxLength(50);

            builder.Property(s => s.IsElective)
                .IsRequired();

            builder.Property(s => s.CycleType)
                .HasMaxLength(50);
        }
    }
}
