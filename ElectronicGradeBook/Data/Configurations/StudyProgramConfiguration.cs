using ElectronicGradeBook.Models.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectronicGradeBook.Data.Configurations
{
    public class StudyProgramConfiguration : IEntityTypeConfiguration<StudyProgram>
    {
        public void Configure(EntityTypeBuilder<StudyProgram> builder)
        {
            builder.ToTable("StudyPrograms");
            builder.HasKey(sp => sp.Id);

            builder.Property(sp => sp.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(sp => sp.DurationYears)
                .IsRequired();
        }
    }
}
