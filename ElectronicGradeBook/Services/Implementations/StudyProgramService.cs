using ElectronicGradeBook.Data;
using ElectronicGradeBook.Models.Entities.Core;
using ElectronicGradeBook.Models.Filters;
using ElectronicGradeBook.Models.ViewModels;
using ElectronicGradeBook.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectronicGradeBook.Services.Implementations
{
    public class StudyProgramService : IStudyProgramService
    {
        private readonly ApplicationDbContext _db;
        public StudyProgramService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<StudyProgramViewModel>> GetAllAsync()
        {
            return await _db.StudyPrograms
                .OrderBy(sp => sp.Name)
                .Select(sp => new StudyProgramViewModel
                {
                    Id = sp.Id,
                    Name = sp.Name,
                    DurationYears = sp.DurationYears
                })
                .ToListAsync();
        }

        public async Task<PagedResult<StudyProgramViewModel>> GetFilteredAsync(StudyProgramFilterModel filter)
        {
            var query = _db.StudyPrograms.AsQueryable();

            if (!string.IsNullOrEmpty(filter.Search))
            {
                query = query.Where(sp => sp.Name.Contains(filter.Search));
            }

            if (filter.DurationYears.HasValue)
            {
                query = query.Where(sp => sp.DurationYears == filter.DurationYears.Value);
            }

            int totalItems = await query.CountAsync();
            int skip = (filter.PageNumber - 1) * filter.PageSize;

            var items = await query
                .OrderBy(sp => sp.Name)
                .Skip(skip)
                .Take(filter.PageSize)
                .Select(sp => new StudyProgramViewModel
                {
                    Id = sp.Id,
                    Name = sp.Name,
                    DurationYears = sp.DurationYears
                })
                .ToListAsync();

            return new PagedResult<StudyProgramViewModel>
            {
                Items = items,
                TotalItems = totalItems,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }


        public async Task<StudyProgramViewModel> CreateAsync(StudyProgramViewModel model)
        {
            // Перевірка унікальності
            bool existName = await _db.StudyPrograms
                .AnyAsync(x => x.Name == model.Name);
            if (existName)
                throw new Exception($"Освітня програма '{model.Name}' вже існує.");

            var entity = new StudyProgram
            {
                Name = model.Name,
                DurationYears = model.DurationYears
            };
            _db.StudyPrograms.Add(entity);
            await _db.SaveChangesAsync();

            model.Id = entity.Id;
            return model;
        }

        public async Task<StudyProgramViewModel> UpdateAsync(StudyProgramViewModel model)
        {
            var sp = await _db.StudyPrograms.FindAsync(model.Id);
            if (sp == null)
                throw new Exception("Програму не знайдено.");

            bool existSame = await _db.StudyPrograms
                .AnyAsync(x => x.Name == model.Name && x.Id != model.Id);
            if (existSame)
                throw new Exception($"Освітня програма '{model.Name}' вже існує.");

            sp.Name = model.Name;
            sp.DurationYears = model.DurationYears;
            await _db.SaveChangesAsync();

            return model;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sp = await _db.StudyPrograms.FindAsync(id);
            if (sp == null)
                throw new Exception("Програму не знайдено.");

            // Перевірка, чи є Specialty
            bool hasSpecialties = await _db.Specialties
                .AnyAsync(s => s.ProgramId == id);
            if (hasSpecialties)
                throw new Exception("Не можна видалити програму — є спеціальності, що посилаються.");

            _db.StudyPrograms.Remove(sp);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
