using ElectronicGradeBook.Models.ViewModels;

namespace ElectronicGradeBook.Services.Interfaces
{
    public interface IFacultyService
    {
        Task<List<FacultyViewModel>> GetAllAsync();
        Task<FacultyViewModel> CreateAsync(FacultyViewModel model);
        Task<FacultyViewModel> UpdateAsync(FacultyViewModel model);
        Task<bool> DeleteAsync(int id);
    }
}