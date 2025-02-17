using ElectronicGradeBook.Models.ViewModels;

namespace ElectronicGradeBook.Services.Interfaces
{
    public interface ISubjectSubgroupService
    {
        // Повертаємо всі підгрупи для певного SubjectOffering
        Task<List<SubjectSubgroupViewModel>> GetByOfferingAsync(int offeringId);

        // Створити підгрупу
        Task<SubjectSubgroupViewModel> CreateAsync(SubjectSubgroupViewModel model);

        // Редагувати
        Task<SubjectSubgroupViewModel> UpdateAsync(SubjectSubgroupViewModel model);

        // Видалити
        Task<bool> DeleteAsync(int id);

        // Додати студента в підгрупу
        Task<bool> AddStudentToSubgroup(int subgroupId, int studentId);

        // Вилучити студента з підгрупи
        Task<bool> RemoveStudentFromSubgroup(int subgroupId, int studentId);
    }
}
