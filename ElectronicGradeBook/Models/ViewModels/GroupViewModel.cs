namespace ElectronicGradeBook.Models.ViewModels
{
    public class GroupViewModel
    {
        public int Id { get; set; }
        public string GroupPrefix { get; set; }
        public int GroupNumber { get; set; }
        public int CurrentStudyYear { get; set; }
        public int EnrollmentYear { get; set; }
        public int GraduationYear { get; set; }

        public int SpecialtyId { get; set; }
        public string SpecialtyName { get; set; } 
        public bool IsGraduated { get; set; }
    }
}
