using ElectronicGradeBook.Models.Entities.ActivityPrivilege;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ElectronicGradeBook.Models.Entities.Core;
using ElectronicGradeBook.Models.Entities.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ElectronicGradeBook.Data.Configurations;


namespace ElectronicGradeBook.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet-и (Core)
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<StudyProgram> StudyPrograms { get; set; }
        public DbSet<Specialty> Specialties { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<SubjectSubgroup> SubjectSubgroups { get; set; }
        public DbSet<SubjectSubgroupStudent> SubjectSubgroupStudents { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<SubjectOffering> SubjectOfferings { get; set; }
        public DbSet<SubjectOfferingGroup> SubjectOfferingGroups { get; set; }
        public DbSet<Grade> Grades { get; set; }

        // DbSet-и (ActivityPrivilege)
        public DbSet<Activity> Activities { get; set; }
        public DbSet<StudentActivity> StudentActivities { get; set; }
        public DbSet<Privilege> Privileges { get; set; }
        public DbSet<StudentPrivilege> StudentPrivileges { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ВАЖЛИВО: викликаємо базовий OnModelCreating, щоб Identity налаштувався
            base.OnModelCreating(modelBuilder);

            // Застосувати усі IEntityTypeConfiguration<>:
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}

