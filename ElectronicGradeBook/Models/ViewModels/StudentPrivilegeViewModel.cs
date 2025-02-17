namespace ElectronicGradeBook.Models.ViewModels
{
    public class StudentPrivilegeViewModel
    {
        public int Id { get; set; }

        public int StudentId { get; set; }
        public string StudentName { get; set; } // опційно

        public int PrivilegeId { get; set; }
        public string PrivilegeName { get; set; } // опційно

        public DateTime DateGranted { get; set; }
        public DateTime? DateRevoked { get; set; }
    }
}
