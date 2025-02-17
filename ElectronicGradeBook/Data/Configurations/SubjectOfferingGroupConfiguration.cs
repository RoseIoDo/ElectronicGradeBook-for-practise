using ElectronicGradeBook.Models.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectronicGradeBook.Data.Configurations
{
    public class SubjectOfferingGroupConfiguration : IEntityTypeConfiguration<SubjectOfferingGroup>
    {
        public void Configure(EntityTypeBuilder<SubjectOfferingGroup> builder)
        {
            builder.ToTable("SubjectOfferingGroups");
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.SubjectOffering)
                .WithMany(o => o.SubjectOfferingGroups)
                .HasForeignKey(x => x.SubjectOfferingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Group)
                .WithMany(g => g.SubjectOfferings)
                .HasForeignKey(x => x.GroupId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
