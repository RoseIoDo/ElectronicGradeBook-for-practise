namespace ElectronicGradeBook.Models.Filters
{
    public class SpecialtyFilterModel
    {
        public string Search { get; set; } // шукати в Name або ShortName
        public int? FacultyId { get; set; }
        public int? ProgramId { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
