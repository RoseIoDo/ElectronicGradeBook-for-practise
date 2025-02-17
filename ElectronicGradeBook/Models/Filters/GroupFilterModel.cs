namespace ElectronicGradeBook.Models.Filters
{
    public class GroupFilterModel
    {
        public string PrefixSearch { get; set; }
        public int? SpecialtyId { get; set; }
        public int? EnrollmentYear { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
