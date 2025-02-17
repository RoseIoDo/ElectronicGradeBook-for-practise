using ElectronicGradeBook.Models.ViewModels;

namespace ElectronicGradeBook.Services.Interfaces
{
    public interface IStudentActivityService
    {
        Task<List<StudentActivityViewModel>> GetAllAsync();
        Task<StudentActivityViewModel> CreateAsync(StudentActivityViewModel model);
        Task<StudentActivityViewModel> UpdateAsync(StudentActivityViewModel model);
        Task<bool> DeleteAsync(int id);
    }
}
