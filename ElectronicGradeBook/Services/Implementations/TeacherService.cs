using ElectronicGradeBook.Data;
using ElectronicGradeBook.Models.Entities.Core;
using ElectronicGradeBook.Models.Filters;
using ElectronicGradeBook.Models.ViewModels;
using ElectronicGradeBook.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ElectronicGradeBook.Services.Implementations
{
    public class TeacherService : ITeacherService
    {
        private readonly ApplicationDbContext _db;
        public TeacherService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<TeacherViewModel>> GetAllAsync()
        {
            return await _db.Teachers
                .OrderBy(t => t.FullName)
                .Select(t => new TeacherViewModel
                {
                    Id = t.Id,
                    FullName = t.FullName,
                    Position = t.Position,
                    //UserId = t.UserId,
                })
                .ToListAsync();
        }

        public async Task<PagedResult<TeacherViewModel>> GetFilteredAsync(TeacherFilterModel filter)
        {
            var query = _db.Teachers
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter.Search))
            {
                query = query.Where(t => t.FullName.Contains(filter.Search));
            }

            if (!string.IsNullOrEmpty(filter.Position))
            {
                query = query.Where(t => t.Position == filter.Position);
            }

            int totalItems = await query.CountAsync();
            int skip = (filter.PageNumber - 1) * filter.PageSize;

            var items = await query
                .OrderBy(t => t.FullName)
                .Skip(skip)
                .Take(filter.PageSize)
                .Select(t => new TeacherViewModel
                {
                    Id = t.Id,
                    FullName = t.FullName,
                    Position = t.Position,
                    //UserId = t.UserId,
                })
                .ToListAsync();

            return new PagedResult<TeacherViewModel>
            {
                Items = items,
                TotalItems = totalItems,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }


        public async Task<TeacherViewModel> CreateAsync(TeacherViewModel model)
        {
            // Можна перевірити унікальність ПІБ + посади
            var entity = new Teacher
            {
                FullName = model.FullName,
                Position = model.Position,
                //UserId = model.UserId
            };
            _db.Teachers.Add(entity);
            await _db.SaveChangesAsync();
            model.Id = entity.Id;
            return model;
        }

        public async Task<TeacherViewModel> UpdateAsync(TeacherViewModel model)
        {
            var teacher = await _db.Teachers.FindAsync(model.Id);
            if (teacher == null)
                throw new Exception("Викладача не знайдено.");

            teacher.FullName = model.FullName;
            teacher.Position = model.Position;
            //teacher.UserId = model.UserId;

            await _db.SaveChangesAsync();
            return model;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var teacher = await _db.Teachers.FindAsync(id);
            if (teacher == null)
                throw new Exception("Викладача не знайдено.");

            // Якщо він має SubjectOfferings
            bool hasOfferings = await _db.SubjectOfferings
                .AnyAsync(o => o.TeacherId == id);
            if (hasOfferings)
                throw new Exception("Не можна видалити викладача — є предмети-викладання.");

            _db.Teachers.Remove(teacher);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
