using ElectronicGradeBook.Models.Entities.Core;

public class SubjectSubgroup
{
    public int Id { get; set; }

    public int SubjectOfferingId { get; set; }
    public SubjectOffering SubjectOffering { get; set; }

    /// <summary>
    /// Назва підгрупи
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// За бажанням у кожної підгрупи може бути свій викладач (або null)
    /// </summary>
    public int? TeacherId { get; set; }
    public Teacher Teacher { get; set; }

    /// <summary>
    /// Студенти, які входять у цю підгрупу
    /// </summary>
    public ICollection<SubjectSubgroupStudent> SubjectSubgroupStudents { get; set; }
        = new List<SubjectSubgroupStudent>();
}
