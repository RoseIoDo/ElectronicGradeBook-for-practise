using ElectronicGradeBook.Models.Entities.Core;

namespace ElectronicGradeBook.Models.Entities.ActivityPrivilege
{
    public class StudentActivity
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }

        public int ActivityId { get; set; }
        public Activity Activity { get; set; }

        public DateTime DateAwarded { get; set; }
        public string Semester { get; set; }
        public string Notes { get; set; }
    }
}
