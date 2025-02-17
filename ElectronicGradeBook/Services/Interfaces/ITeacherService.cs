using ElectronicGradeBook.Models.Filters;
using ElectronicGradeBook.Models.ViewModels;

namespace ElectronicGradeBook.Services.Interfaces
{
    public interface ITeacherService
    {
        Task<List<TeacherViewModel>> GetAllAsync();
        Task<TeacherViewModel> CreateAsync(TeacherViewModel model);
        Task<TeacherViewModel> UpdateAsync(TeacherViewModel model);
        Task<bool> DeleteAsync(int id);
        Task<PagedResult<TeacherViewModel>> GetFilteredAsync(TeacherFilterModel filter);
    }
}
