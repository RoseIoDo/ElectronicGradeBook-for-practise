namespace ElectronicGradeBook.Models.Entities.Core
{
    public class Student
    {
        public int Id { get; set; }

        // Якщо студент може мати обліковку користувача
        public int? UserId { get; set; }
        public string FullName { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public DateTime? GraduationDate { get; set; }
        public bool IsActive { get; set; }

        public int GroupId { get; set; }
        public Group Group { get; set; }

        public ICollection<Grade> Grades { get; set; } = new List<Grade>();
    }
}
