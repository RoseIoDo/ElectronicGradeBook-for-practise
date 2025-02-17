using ElectronicGradeBook.Models.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectronicGradeBook.Data.Configurations
{
    public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
    {
        public void Configure(EntityTypeBuilder<Teacher> builder)
        {
            builder.ToTable("Teachers");
            builder.HasKey(t => t.Id);

            builder.Property(t => t.FullName)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(t => t.Position)
                .HasMaxLength(100)
                .IsRequired();

        }
    }
}
