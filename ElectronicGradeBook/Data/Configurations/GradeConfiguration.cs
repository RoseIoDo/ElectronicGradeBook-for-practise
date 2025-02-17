using ElectronicGradeBook.Models.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectronicGradeBook.Data.Configurations
{
    public class GradeConfiguration : IEntityTypeConfiguration<Grade>
    {
        public void Configure(EntityTypeBuilder<Grade> builder)
        {
            builder.ToTable("Grades");
            builder.HasKey(g => g.Id);

            builder.Property(g => g.GradeVersionJson)
                .HasColumnType("nvarchar(max)");

            builder.Property(g => g.Status)
                .HasMaxLength(50);

            builder.Property(g => g.DateUpdated)
                .IsRequired();

            builder.HasOne(g => g.SubjectOffering)
                .WithMany(o => o.Grades)
                .HasForeignKey(g => g.SubjectOfferingId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(g => g.Student)
                .WithMany(s => s.Grades)
                .HasForeignKey(g => g.StudentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
