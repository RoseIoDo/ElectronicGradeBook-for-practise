using ElectronicGradeBook.Data;
using ElectronicGradeBook.Models.Entities.Core;
using ElectronicGradeBook.Models.Filters;
using ElectronicGradeBook.Models.ViewModels;
using ElectronicGradeBook.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ElectronicGradeBook.Services.Implementations
{
    public class SpecialtyService : ISpecialtyService
    {
        private readonly ApplicationDbContext _db;
        public SpecialtyService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<SpecialtyViewModel>> GetAllAsync()
        {
            return await _db.Specialties
                .Include(s => s.Faculty)
                .Include(s => s.StudyProgram)
                .OrderBy(s => s.Name)
                .Select(s => new SpecialtyViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    ShortName = s.ShortName,
                    Code = s.Code,
                    FacultyId = s.FacultyId,
                    FacultyName = s.Faculty.Name,
                    ProgramId = s.ProgramId,
                    ProgramName = s.StudyProgram.Name
                })
                .ToListAsync();
        }

        public async Task<PagedResult<SpecialtyViewModel>> GetFilteredAsync(SpecialtyFilterModel filter)
        {
            var query = _db.Specialties
                .Include(s => s.Faculty)
                .Include(s => s.StudyProgram)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter.Search))
            {
                query = query.Where(s => s.Name.Contains(filter.Search)
                                      || s.ShortName.Contains(filter.Search));
            }

            if (filter.FacultyId.HasValue && filter.FacultyId.Value > 0)
            {
                query = query.Where(s => s.FacultyId == filter.FacultyId.Value);
            }

            if (filter.ProgramId.HasValue && filter.ProgramId.Value > 0)
            {
                query = query.Where(s => s.ProgramId == filter.ProgramId.Value);
            }

            int totalItems = await query.CountAsync();

            int skip = (filter.PageNumber - 1) * filter.PageSize;
            var items = await query
                .OrderBy(s => s.Name)
                .Skip(skip)
                .Take(filter.PageSize)
                .Select(s => new SpecialtyViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    ShortName = s.ShortName,
                    Code = s.Code,
                    FacultyId = s.FacultyId,
                    FacultyName = s.Faculty.Name,
                    ProgramId = s.ProgramId,
                    ProgramName = s.StudyProgram.Name
                })
                .ToListAsync();

            return new PagedResult<SpecialtyViewModel>
            {
                Items = items,
                TotalItems = totalItems,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<SpecialtyViewModel> CreateAsync(SpecialtyViewModel model)
        {
            // Перевірка унікального коду
            bool existCode = await _db.Specialties
                .AnyAsync(x => x.Code == model.Code);
            if (existCode)
                throw new Exception($"Спеціальність із кодом {model.Code} вже існує.");

            var entity = new Specialty
            {
                Name = model.Name,
                ShortName = model.ShortName,
                Code = model.Code,
                FacultyId = model.FacultyId,
                ProgramId = model.ProgramId
            };
            _db.Specialties.Add(entity);
            await _db.SaveChangesAsync();

            model.Id = entity.Id;
            return model;
        }

        public async Task<SpecialtyViewModel> UpdateAsync(SpecialtyViewModel model)
        {
            var spec = await _db.Specialties.FindAsync(model.Id);
            if (spec == null)
                throw new Exception("Спеціальність не знайдено.");

            bool existSameCode = await _db.Specialties
                .AnyAsync(x => x.Code == model.Code && x.Id != model.Id);
            if (existSameCode)
                throw new Exception($"Спеціальність із кодом {model.Code} вже існує.");

            spec.Name = model.Name;
            spec.ShortName = model.ShortName;
            spec.Code = model.Code;
            spec.FacultyId = model.FacultyId;
            spec.ProgramId = model.ProgramId;
            await _db.SaveChangesAsync();

            return model;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var spec = await _db.Specialties.FindAsync(id);
            if (spec == null)
                throw new Exception("Не знайдено спеціальність.");

            // Перевірка, чи є Group
            bool hasGroups = await _db.Groups
                .AnyAsync(g => g.SpecialtyId == id);
            if (hasGroups)
                throw new Exception("Не можна видалити спеціальність — є групи з нею.");

            _db.Specialties.Remove(spec);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
