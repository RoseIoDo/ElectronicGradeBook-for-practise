namespace ElectronicGradeBook.Models.ViewModels
{
    public class SpecialtyViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public int Code { get; set; }

        public int FacultyId { get; set; }
        public string FacultyName { get; set; } // опційно, щоб показати у випадаючому списку

        public int ProgramId { get; set; }
        public string ProgramName { get; set; } // опційно

        // тривалість освітньої програми (у роках)
        public int StudyProgramDuration { get; set; }
    }
}
