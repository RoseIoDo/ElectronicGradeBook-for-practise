using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ElectronicGradeBook.Data
{
    public class DesignTimeDbContextFactory
    : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connectionString = "Server=localhost;Database=ElectronicGradeBook;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
            builder.UseSqlServer(connectionString);

            return new ApplicationDbContext(builder.Options);
        }
    }

}
