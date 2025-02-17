using ElectronicGradeBook.Models.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectronicGradeBook.Data.Configurations
{
    public class FacultyConfiguration : IEntityTypeConfiguration<Faculty>
    {
        public void Configure(EntityTypeBuilder<Faculty> builder)
        {
            builder.ToTable("Faculties");
            builder.HasKey(f => f.Id);

            builder.Property(f => f.Name)
                .HasMaxLength(200)
                .IsRequired();

            // Зв’язок із Specialty
            builder.HasMany(f => f.Specialties)
                .WithOne(s => s.Faculty)
                .HasForeignKey(s => s.FacultyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
