using ElectronicGradeBook.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using ElectronicGradeBook.Models.Filters;

namespace ElectronicGradeBook.Services.Interfaces
{
    public interface IStudyProgramService
    {
        Task<List<StudyProgramViewModel>> GetAllAsync();
        Task<StudyProgramViewModel> CreateAsync(StudyProgramViewModel model);
        Task<StudyProgramViewModel> UpdateAsync(StudyProgramViewModel model);
        Task<bool> DeleteAsync(int id);
        Task<PagedResult<StudyProgramViewModel>> GetFilteredAsync(StudyProgramFilterModel filter);

    }
}