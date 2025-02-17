using ElectronicGradeBook.Models.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectronicGradeBook.Data.Configurations
{
    public class SubjectSubgroupConfiguration : IEntityTypeConfiguration<SubjectSubgroup>
    {
        public void Configure(EntityTypeBuilder<SubjectSubgroup> builder)
        {
            builder.ToTable("SubjectSubgroups");
            builder.HasKey(sg => sg.Id);

            builder.Property(sg => sg.Name)
                .HasMaxLength(100)
                .IsRequired();

            // Кожна підгрупа пов’язана з одним SubjectOffering (OnDelete.Cascade)
            builder.HasOne(sg => sg.SubjectOffering)
                .WithMany(o => o.SubjectSubgroups)
                .HasForeignKey(sg => sg.SubjectOfferingId)
                .OnDelete(DeleteBehavior.Cascade);

            // Якщо треба вказувати окремого викладача для підгрупи
            builder.HasOne(sg => sg.Teacher)
                .WithMany() // або .WithMany(t => t.SubjectSubgroups) якщо треба
                .HasForeignKey(sg => sg.TeacherId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
