namespace ElectronicGradeBook.Models.Entities.Core
{
    public class Group
    {
        public int Id { get; set; }
        public int SpecialtyId { get; set; }
        public Specialty Specialty { get; set; }

        public string GroupPrefix { get; set; }
        public int GroupNumber { get; set; }
        public int CurrentStudyYear { get; set; }
        public int SubgroupNumber { get; set; }
        public int EnrollmentYear { get; set; }
        public int GraduationYear { get; set; }

        public ICollection<Student> Students { get; set; } = new List<Student>();
        public ICollection<SubjectOfferingGroup> SubjectOfferings { get; set; } = new List<SubjectOfferingGroup>();
    }
}
