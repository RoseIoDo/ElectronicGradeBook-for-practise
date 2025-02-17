namespace ElectronicGradeBook.Models.Entities.ActivityPrivilege
{
    public class Activity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Points { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }

        public ICollection<StudentActivity> StudentActivities { get; set; } = new List<StudentActivity>();
    }
}
