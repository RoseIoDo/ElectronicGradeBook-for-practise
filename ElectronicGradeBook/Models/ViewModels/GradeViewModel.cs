namespace ElectronicGradeBook.Models.ViewModels
{
    public class GradeViewModel
    {
        public int Id { get; set; }

        public int SubjectOfferingId { get; set; }
        public string SubjectName { get; set; } // опційно
        public int StudentId { get; set; }
        public string StudentName { get; set; } // опційно

        public string GradeVersionJson { get; set; }
        public string Status { get; set; }
        public bool IsRetake { get; set; }
        public DateTime DateUpdated { get; set; }
    }
}
