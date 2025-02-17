using ElectronicGradeBook.Models.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectronicGradeBook.Data.Configurations
{
    public class SubjectOfferingConfiguration : IEntityTypeConfiguration<SubjectOffering>
    {
        public void Configure(EntityTypeBuilder<SubjectOffering> builder)
        {
            builder.ToTable("SubjectOfferings");
            builder.HasKey(o => o.Id);

            builder.Property(o => o.AcademicYear).HasMaxLength(20);
            builder.Property(o => o.Credits).HasColumnType("decimal(4,1)").IsRequired();

            builder.HasOne(o => o.Subject)
                .WithMany(s => s.SubjectOfferings)
                .HasForeignKey(o => o.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.Teacher)
                .WithMany(t => t.SubjectOfferings)
                .HasForeignKey(o => o.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
