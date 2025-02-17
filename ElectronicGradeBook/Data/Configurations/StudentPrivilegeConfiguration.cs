using ElectronicGradeBook.Models.Entities.ActivityPrivilege;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectronicGradeBook.Data.Configurations
{
    public class StudentPrivilegeConfiguration : IEntityTypeConfiguration<StudentPrivilege>
    {
        public void Configure(EntityTypeBuilder<StudentPrivilege> builder)
        {
            builder.ToTable("StudentPrivileges");
            builder.HasKey(sp => sp.Id);

            builder.Property(sp => sp.DateGranted)
                .IsRequired();

            builder.HasOne(sp => sp.Privilege)
                .WithMany(p => p.StudentPrivileges)
                .HasForeignKey(sp => sp.PrivilegeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
