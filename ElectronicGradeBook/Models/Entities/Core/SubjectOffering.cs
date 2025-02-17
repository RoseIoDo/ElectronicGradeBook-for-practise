namespace ElectronicGradeBook.Models.Entities.Core
{
    public class SubjectOffering
    {
        public int Id { get; set; }

        public int SubjectId { get; set; }
        public Subject Subject { get; set; }

        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }

        public int YearOfStudy { get; set; }
        public int SemesterInYear { get; set; }
        public string AcademicYear { get; set; }
        public decimal Credits { get; set; }

        // Проміжна таблиця "SubjectOfferingGroup"
        public ICollection<SubjectOfferingGroup> SubjectOfferingGroups { get; set; }
            = new List<SubjectOfferingGroup>();

        // Оцінки
        public ICollection<Grade> Grades { get; set; } = new List<Grade>();

        // Підгрупи (SubjectSubgroup)
        public ICollection<SubjectSubgroup> SubjectSubgroups { get; set; } = new List<SubjectSubgroup>();
    }
}
