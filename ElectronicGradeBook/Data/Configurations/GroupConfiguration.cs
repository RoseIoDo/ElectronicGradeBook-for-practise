using ElectronicGradeBook.Models.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectronicGradeBook.Data.Configurations
{
    public class GroupConfiguration : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.ToTable("Groups");
            builder.HasKey(g => g.Id);

            builder.Property(g => g.GroupPrefix)
                .HasMaxLength(20);

            builder.Property(g => g.GroupNumber)
                .IsRequired();

            builder.Property(g => g.CurrentStudyYear)
                .IsRequired();

            builder.Property(g => g.EnrollmentYear)
                .IsRequired();

            builder.Property(g => g.GraduationYear)
                .IsRequired();

            builder.HasOne(g => g.Specialty)
                .WithMany(s => s.Groups)
                .HasForeignKey(g => g.SpecialtyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
