using ElectronicGradeBook.Models.Entities.Core;

namespace ElectronicGradeBook.Models.Entities.ActivityPrivilege
{
    public class StudentPrivilege
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }

        public int PrivilegeId { get; set; }
        public Privilege Privilege { get; set; }

        public DateTime DateGranted { get; set; }
        public DateTime? DateRevoked { get; set; }
    }
}
