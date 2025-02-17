namespace ElectronicGradeBook.Models.Filters
{
    public class StudyProgramFilterModel
    {
        public string Search { get; set; }  // Пошук по назві
        public int? DurationYears { get; set; } // якщо хочемо фільтр за роками (опц.)

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
