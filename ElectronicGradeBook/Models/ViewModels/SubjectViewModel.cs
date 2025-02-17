namespace ElectronicGradeBook.Models.ViewModels
{
    public class SubjectViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string ShortName { get; set; }
        public string Code { get; set; }
        public bool IsElective { get; set; }
        public string CycleType { get; set; }
    }
}
