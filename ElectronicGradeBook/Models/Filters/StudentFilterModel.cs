namespace ElectronicGradeBook.Models.Filters
{
    public class StudentFilterModel
    {
        public string Search { get; set; }       // пошук по FullName
        public int? GroupId { get; set; }        // фільтр за групою
        public bool? IsActive { get; set; }      // фільтр за статусом

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
