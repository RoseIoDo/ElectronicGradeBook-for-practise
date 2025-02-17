namespace ElectronicGradeBook.Models.Filters
{
    public class SubjectOfferingFilterModel
    {
        public int? SubjectId { get; set; }
        public int? TeacherId { get; set; }
        public int? GroupId { get; set; }
        public int? SemesterInYear { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
