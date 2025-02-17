namespace ElectronicGradeBook.Models.Entities.Core
{
    public class Subject
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string ShortName { get; set; }
        public string Code { get; set; }
        public bool IsElective { get; set; }
        public string CycleType { get; set; }

        public ICollection<SubjectOffering> SubjectOfferings { get; set; } = new List<SubjectOffering>();
    }
}
