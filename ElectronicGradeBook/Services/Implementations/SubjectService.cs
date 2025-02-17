using ElectronicGradeBook.Data;
using ElectronicGradeBook.Models.Entities.Core;
using ElectronicGradeBook.Models.Filters;
using ElectronicGradeBook.Models.ViewModels;
using ElectronicGradeBook.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ElectronicGradeBook.Services.Implementations
{
    public class SubjectService : ISubjectService
    {
        private readonly ApplicationDbContext _db;
        public SubjectService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<SubjectViewModel>> GetAllAsync()
        {
            return await _db.Subjects
                .OrderBy(s => s.FullName)
                .Select(s => new SubjectViewModel
                {
                    Id = s.Id,
                    FullName = s.FullName,
                    ShortName = s.ShortName,
                    Code = s.Code,
                    IsElective = s.IsElective,
                    CycleType = s.CycleType
                })
                .ToListAsync();
        }

        public async Task<PagedResult<SubjectViewModel>> GetFilteredAsync(SubjectFilterModel filter)
        {
            var query = _db.Subjects.AsQueryable();

            if (!string.IsNullOrEmpty(filter.Search))
            {
                query = query.Where(s => s.FullName.Contains(filter.Search)
                                      || s.ShortName.Contains(filter.Search));
            }

            if (filter.IsElective.HasValue)
            {
                query = query.Where(s => s.IsElective == filter.IsElective.Value);
            }

            int totalItems = await query.CountAsync();

            var skip = (filter.PageNumber - 1) * filter.PageSize;
            var items = await query
                .OrderBy(s => s.FullName)
                .Skip(skip)
                .Take(filter.PageSize)
                .Select(s => new SubjectViewModel
                {
                    Id = s.Id,
                    FullName = s.FullName,
                    ShortName = s.ShortName,
                    Code = s.Code,
                    IsElective = s.IsElective,
                    CycleType = s.CycleType
                })
                .ToListAsync();

            return new PagedResult<SubjectViewModel>
            {
                Items = items,
                TotalItems = totalItems,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<SubjectViewModel> CreateAsync(SubjectViewModel model)
        {
            // Перевірка унікальності Code
            var entity = new Subject
            {
                FullName = model.FullName,
                ShortName = model.ShortName,
                Code = model.Code,
                IsElective = model.IsElective,
                CycleType = model.CycleType
            };
            _db.Subjects.Add(entity);
            await _db.SaveChangesAsync();
            model.Id = entity.Id;
            return model;
        }

        public async Task<SubjectViewModel> UpdateAsync(SubjectViewModel model)
        {
            var sbj = await _db.Subjects.FindAsync(model.Id);
            if (sbj == null)
                throw new Exception("Предмет не знайдено.");

            sbj.FullName = model.FullName;
            sbj.ShortName = model.ShortName;
            sbj.Code = model.Code;
            sbj.IsElective = model.IsElective;
            sbj.CycleType = model.CycleType;

            await _db.SaveChangesAsync();
            return model;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sbj = await _db.Subjects.FindAsync(id);
            if (sbj == null)
                throw new Exception("Предмет не знайдено.");

            bool hasOfferings = await _db.SubjectOfferings
                .AnyAsync(o => o.SubjectId == id);
            if (hasOfferings)
                throw new Exception("Не можна видалити — є SubjectOfferings.");

            _db.Subjects.Remove(sbj);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
