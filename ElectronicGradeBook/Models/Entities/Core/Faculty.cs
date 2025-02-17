namespace ElectronicGradeBook.Models.Entities.Core
{
    public class Faculty
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Навігація
        public ICollection<Specialty> Specialties { get; set; } = new List<Specialty>();
    }
}
