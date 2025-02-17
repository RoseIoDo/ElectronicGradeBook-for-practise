using ElectronicGradeBook.Models.ViewModels;

namespace ElectronicGradeBook.Services.Interfaces
{
    public interface IPrivilegeService
    {
        Task<List<PrivilegeViewModel>> GetAllAsync();
        Task<PrivilegeViewModel> CreateAsync(PrivilegeViewModel model);
        Task<PrivilegeViewModel> UpdateAsync(PrivilegeViewModel model);
        Task<bool> DeleteAsync(int id);
    }
}
