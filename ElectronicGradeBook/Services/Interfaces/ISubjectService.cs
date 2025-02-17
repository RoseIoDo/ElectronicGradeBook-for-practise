using ElectronicGradeBook.Models.Filters;
using ElectronicGradeBook.Models.ViewModels;

namespace ElectronicGradeBook.Services.Interfaces
{
    public interface ISubjectService
    {
        Task<List<SubjectViewModel>> GetAllAsync();
        Task<SubjectViewModel> CreateAsync(SubjectViewModel model);
        Task<SubjectViewModel> UpdateAsync(SubjectViewModel model);
        Task<bool> DeleteAsync(int id);
        Task<PagedResult<SubjectViewModel>> GetFilteredAsync(SubjectFilterModel filter);

    }
}
