namespace ElectronicGradeBook.Models.Entities.Core
{
    public class Grade
    {
        public int Id { get; set; }

        public int SubjectOfferingId { get; set; }
        public SubjectOffering SubjectOffering { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; }

        public string GradeVersionJson { get; set; }  // Зберігаємо JSON з різними даними 
        public string Status { get; set; }
        public DateTime DateUpdated { get; set; }
        public bool IsRetake { get; set; }
    }
}
