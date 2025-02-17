namespace ElectronicGradeBook.Models.Filters
{
    public class TeacherFilterModel
    {
        public string Search { get; set; }  // пошук по FullName
        public string Position { get; set; } // фільтр за назвою посади

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
