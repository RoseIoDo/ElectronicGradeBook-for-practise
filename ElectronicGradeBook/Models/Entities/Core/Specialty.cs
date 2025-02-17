namespace ElectronicGradeBook.Models.Entities.Core
{
    public class Specialty
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public int Code { get; set; }

        public int FacultyId { get; set; }
        public Faculty Faculty { get; set; }

        public int ProgramId { get; set; }
        public StudyProgram StudyProgram { get; set; }

        public ICollection<Group> Groups { get; set; } = new List<Group>();
    }
}
