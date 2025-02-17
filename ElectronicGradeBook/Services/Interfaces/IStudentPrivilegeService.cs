using ElectronicGradeBook.Models.ViewModels;

namespace ElectronicGradeBook.Services.Interfaces
{
    public interface IStudentPrivilegeService
    {
        Task<List<StudentPrivilegeViewModel>> GetAllAsync();
        Task<StudentPrivilegeViewModel> CreateAsync(StudentPrivilegeViewModel model);
        Task<StudentPrivilegeViewModel> UpdateAsync(StudentPrivilegeViewModel model);
        Task<bool> DeleteAsync(int id);
    }
}
