using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ElectronicGradeBook.Models.ViewModels;

namespace ElectronicGradeBook.Services.Interfaces
{
    public interface IGradeBookService
    {
        Task<List<FacultyViewModel>> GetFacultiesAsync();
        Task<List<SpecialtyViewModel>> GetSpecialtiesAsync(int facultyId);
        Task<List<GroupViewModel>> GetGroupsAsync(int specialtyId);
        Task<List<SubjectOfferingViewModel>> GetSubjectOfferingsAsync(string groupVal);
        Task<List<StudentGradeDto>> GetGradesForOfferingAsync(int offId);
        Task UpdateGradeAsync(int studentId, int subjectOfferingId, decimal? points, bool isRetake);
        Task<List<GradeVersionItem>> GetGradeHistoryAsync(int studentId, int offeringId);
    }

    // DTO для відображення оцінок
    public class StudentGradeDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public decimal? Points { get; set; }
        public bool IsRetake { get; set; }
    }

    // DTO для зберігання версій оцінок
    public class GradeVersionItem
    {
        public decimal? Points { get; set; }
        public bool IsRetake { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
