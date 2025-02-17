namespace ElectronicGradeBook.Models.Entities.Core
{
    /// <summary>
    /// Зв’язка: один SubjectOffering може включати кілька офіційних груп.
    /// Одна офіційна група може фігурувати в кількох SubjectOfferings.
    /// </summary>
    public class SubjectOfferingGroup
    {
        public int Id { get; set; }

        public int SubjectOfferingId { get; set; }
        public SubjectOffering SubjectOffering { get; set; }

        public int GroupId { get; set; }
        public Group Group { get; set; }
    }
}
