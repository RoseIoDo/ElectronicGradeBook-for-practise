using ElectronicGradeBook.Models.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectronicGradeBook.Data.Configurations
{
    public class SubjectSubgroupStudentConfiguration : IEntityTypeConfiguration<SubjectSubgroupStudent>
    {
        public void Configure(EntityTypeBuilder<SubjectSubgroupStudent> builder)
        {
            builder.ToTable("SubjectSubgroupStudents");
            builder.HasKey(sss => sss.Id);

            // Зв’язок із SubjectSubgroup (OnDelete.Cascade)
            builder.HasOne(sss => sss.SubjectSubgroup)
                .WithMany(sg => sg.SubjectSubgroupStudents)
                .HasForeignKey(sss => sss.SubjectSubgroupId)
                .OnDelete(DeleteBehavior.Cascade);

            // Зв’язок зі Student
            builder.HasOne(sss => sss.Student)
                .WithMany() // або .WithMany(st => st.SubjectSubgroupStudents) якщо треба
                .HasForeignKey(sss => sss.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
