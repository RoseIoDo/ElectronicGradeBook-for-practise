using ElectronicGradeBook.Models.Entities.Core;

public class SubjectSubgroupStudent
{
    public int Id { get; set; }

    public int SubjectSubgroupId { get; set; }
    public SubjectSubgroup SubjectSubgroup { get; set; }

    public int StudentId { get; set; }
    public Student Student { get; set; }
}
