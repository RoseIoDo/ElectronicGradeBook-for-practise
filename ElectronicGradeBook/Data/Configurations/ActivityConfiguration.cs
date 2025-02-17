using ElectronicGradeBook.Models.Entities.ActivityPrivilege;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectronicGradeBook.Data.Configurations
{
    public class ActivityConfiguration : IEntityTypeConfiguration<Activity>
    {
        public void Configure(EntityTypeBuilder<Activity> builder)
        {
            builder.ToTable("Activities");
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(a => a.Points)
                .IsRequired();

            builder.Property(a => a.Type)
                .HasMaxLength(100);

            builder.Property(a => a.Description)
                .HasMaxLength(500);
        }
    }
}
