using ElectronicGradeBook.Models.Filters;
using ElectronicGradeBook.Models.ViewModels;

namespace ElectronicGradeBook.Services.Interfaces
{
    public interface ISpecialtyService
    {
        Task<List<SpecialtyViewModel>> GetAllAsync();
        Task<SpecialtyViewModel> CreateAsync(SpecialtyViewModel model);
        Task<SpecialtyViewModel> UpdateAsync(SpecialtyViewModel model);
        Task<bool> DeleteAsync(int id);

        Task<PagedResult<SpecialtyViewModel>> GetFilteredAsync(SpecialtyFilterModel filter);

    }
}
