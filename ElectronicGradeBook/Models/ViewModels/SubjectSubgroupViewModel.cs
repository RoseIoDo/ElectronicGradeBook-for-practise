namespace ElectronicGradeBook.Models.ViewModels
{
    public class SubjectSubgroupViewModel
    {
        public int Id { get; set; }
        public int SubjectOfferingId { get; set; }

        public string Name { get; set; }

        public int? TeacherId { get; set; }
        public string TeacherName { get; set; }
    }
}
