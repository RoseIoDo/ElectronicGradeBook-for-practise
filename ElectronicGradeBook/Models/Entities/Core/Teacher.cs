using ElectronicGradeBook.Models.Entities.Security;

namespace ElectronicGradeBook.Models.Entities.Core
{
    public class Teacher
    {
        public int Id { get; set; }
        public int? UserId { get; set; }

        public ApplicationUser User { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }

        public ICollection<SubjectOffering> SubjectOfferings { get; set; } = new List<SubjectOffering>();
    }
}
