using ElectronicGradeBook.Models.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectronicGradeBook.Data.Configurations
{
    public class SpecialtyConfiguration : IEntityTypeConfiguration<Specialty>
    {
        public void Configure(EntityTypeBuilder<Specialty> builder)
        {
            builder.ToTable("Specialties");
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(s => s.ShortName)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(s => s.Code)
                .IsRequired();

            builder.HasOne(s => s.Faculty)
                .WithMany(f => f.Specialties)
                .HasForeignKey(s => s.FacultyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.StudyProgram)
                .WithMany(sp => sp.Specialties)
                .HasForeignKey(s => s.ProgramId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
