namespace ElectronicGradeBook.Models.ViewModels
{
    public class StudentActivityViewModel
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } // опційно

        public int ActivityId { get; set; }
        public string ActivityName { get; set; } // опційно

        public DateTime DateAwarded { get; set; }
        public string Semester { get; set; }
        public string Notes { get; set; }
    }
}
