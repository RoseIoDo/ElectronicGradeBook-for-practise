using ElectronicGradeBook.Models.Entities.ActivityPrivilege;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectronicGradeBook.Data.Configurations
{
    public class StudentActivityConfiguration : IEntityTypeConfiguration<StudentActivity>
    {
        public void Configure(EntityTypeBuilder<StudentActivity> builder)
        {
            builder.ToTable("StudentActivities");
            builder.HasKey(sa => sa.Id);

            builder.Property(sa => sa.DateAwarded)
                .IsRequired();

            builder.Property(sa => sa.Semester)
                .HasMaxLength(50);

            builder.Property(sa => sa.Notes)
                .HasMaxLength(500);

            builder.HasOne(sa => sa.Activity)
                .WithMany(a => a.StudentActivities)
                .HasForeignKey(sa => sa.ActivityId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
