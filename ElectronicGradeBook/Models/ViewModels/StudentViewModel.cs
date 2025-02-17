namespace ElectronicGradeBook.Models.ViewModels
{
    public class StudentViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public DateTime? GraduationDate { get; set; }
        public bool IsActive { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; }

        // Додаткові дані для групування
        public string GroupPrefix { get; set; }
        public int GroupNumber { get; set; }
        public int GroupEnrollmentYear { get; set; }
        public int GroupGraduationYear { get; set; }
        public int GroupCurrentStudyYear { get; set; }
        public string SpecialtyName { get; set; }
    }
}
