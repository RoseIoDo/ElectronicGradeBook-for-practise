using ElectronicGradeBook.Models.ViewModels;

namespace ElectronicGradeBook.Services.Interfaces
{
    public interface IActivityService
    {
        Task<List<ActivityViewModel>> GetAllAsync();
        Task<ActivityViewModel> CreateAsync(ActivityViewModel model);
        Task<ActivityViewModel> UpdateAsync(ActivityViewModel model);
        Task<bool> DeleteAsync(int id);
    }
}
