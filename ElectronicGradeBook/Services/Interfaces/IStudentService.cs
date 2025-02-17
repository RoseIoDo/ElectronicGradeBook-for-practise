using ElectronicGradeBook.Models.Filters;
using ElectronicGradeBook.Models.ViewModels;

namespace ElectronicGradeBook.Services.Interfaces
{
    public interface IStudentService
    {
        Task<List<StudentViewModel>> GetAllAsync();
        Task<StudentViewModel> CreateAsync(StudentViewModel model);
        Task<StudentViewModel> UpdateAsync(StudentViewModel model);
        Task<bool> DeleteAsync(int id);

        Task<PagedResult<StudentViewModel>> GetFilteredAsync(StudentFilterModel filter);
    }
}
