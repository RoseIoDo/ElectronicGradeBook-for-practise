namespace ElectronicGradeBook.Models.ViewModels
{
    public class SubjectOfferingViewModel
    {
        public int Id { get; set; }
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }

        public bool IsSubjectElective { get; set; }

        public int TeacherId { get; set; }
        public string TeacherName { get; set; }

        // Якщо користувач обирає одну групу
        public int? SingleGroupId { get; set; }

        // Якщо користувач вводить “course”:
        public string GroupNameOrCourse { get; set; }

        public int YearOfStudy { get; set; }
        public int SemesterInYear { get; set; }
        public string AcademicYear { get; set; }
        public decimal Credits { get; set; }

        // Для відображення в таблиці: список усіх груп, прив’язаних до цього Offering
        public List<string> CombinedGroupNames { get; set; } = new List<string>();
    }
}
