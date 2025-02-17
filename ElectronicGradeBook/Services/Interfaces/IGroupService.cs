using ElectronicGradeBook.Models.ViewModels;
using ElectronicGradeBook.Models.Filters;

namespace ElectronicGradeBook.Services.Interfaces
{
    public interface IGroupService
    {

        Task<List<GroupViewModel>> GetAllAsync();
        Task<GroupViewModel> CreateAsync(GroupViewModel model);
        Task<GroupViewModel> UpdateAsync(GroupViewModel model);
        Task<bool> DeleteAsync(int id);

        Task<PagedResult<GroupViewModel>> GetFilteredAsync(GroupFilterModel filter);
        Task UpdateGroupsForAcademicDateAsync(DateTime currentDate);

    }
}
