using Microsoft.AspNetCore.Identity;

namespace ElectronicGradeBook.Models.Entities.Security
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public bool IsActive { get; set; }
    }
}
