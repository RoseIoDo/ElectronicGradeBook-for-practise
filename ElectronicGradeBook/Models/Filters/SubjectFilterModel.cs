namespace ElectronicGradeBook.Models.Filters
{
    public class SubjectFilterModel
    {
        public string Search { get; set; }      // fullName or shortName
        public bool? IsElective { get; set; }   // фільтр за типом
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
