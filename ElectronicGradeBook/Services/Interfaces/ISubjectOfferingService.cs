using ElectronicGradeBook.Models.ViewModels;
using ElectronicGradeBook.Models.Filters;

namespace ElectronicGradeBook.Services.Interfaces
{
    public interface ISubjectOfferingService
    {
        Task<List<SubjectOfferingViewModel>> GetAllAsync();
        Task<PagedResult<SubjectOfferingViewModel>> GetFilteredAsync(SubjectOfferingFilterModel filter);

        Task<SubjectOfferingViewModel> CreateAsync(SubjectOfferingViewModel model);
        Task<SubjectOfferingViewModel> UpdateAsync(SubjectOfferingViewModel model);
        Task<bool> DeleteAsync(int id);
    }
}
