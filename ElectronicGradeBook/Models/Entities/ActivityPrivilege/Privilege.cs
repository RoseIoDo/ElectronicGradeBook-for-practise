namespace ElectronicGradeBook.Models.Entities.ActivityPrivilege
{
    public class Privilege
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<StudentPrivilege> StudentPrivileges { get; set; } = new List<StudentPrivilege>();
    }
}
