using ElectronicGradeBook.Models.ViewModels;

namespace ElectronicGradeBook.Services.Interfaces
{
    public interface IGradeService
    {
        Task<List<GradeViewModel>> GetAllAsync();
        Task<GradeViewModel> CreateAsync(GradeViewModel model);
        Task<GradeViewModel> UpdateAsync(GradeViewModel model);
        Task<bool> DeleteAsync(int id);
    }
}
