namespace ElectronicGradeBook.Models.Entities.Core
{
    public class StudyProgram
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DurationYears { get; set; }

        public ICollection<Specialty> Specialties { get; set; } = new List<Specialty>();
    }
}
