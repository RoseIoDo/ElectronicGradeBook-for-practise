using ElectronicGradeBook.Data;
using ElectronicGradeBook.Models.Entities.ActivityPrivilege;
using ElectronicGradeBook.Models.ViewModels;
using ElectronicGradeBook.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ElectronicGradeBook.Services.Implementations
{
    public class ActivityService : IActivityService
    {
        private readonly ApplicationDbContext _db;
        public ActivityService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<ActivityViewModel>> GetAllAsync()
        {
            return await _db.Activities
                .OrderBy(a => a.Name)
                .Select(a => new ActivityViewModel
                {
                    Id = a.Id,
                    Name = a.Name,
                    Points = a.Points,
                    Type = a.Type,
                    Description = a.Description
                })
                .ToListAsync();
        }

        public async Task<ActivityViewModel> CreateAsync(ActivityViewModel model)
        {
            // Можливо, унікальність Name?
            var entity = new Activity
            {
                Name = model.Name,
                Points = model.Points,
                Type = model.Type,
                Description = model.Description
            };
            _db.Activities.Add(entity);
            await _db.SaveChangesAsync();
            model.Id = entity.Id;
            return model;
        }

        public async Task<ActivityViewModel> UpdateAsync(ActivityViewModel model)
        {
            var act = await _db.Activities.FindAsync(model.Id);
            if (act == null)
                throw new Exception("Активність не знайдено.");

            act.Name = model.Name;
            act.Points = model.Points;
            act.Type = model.Type;
            act.Description = model.Description;
            await _db.SaveChangesAsync();
            return model;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var act = await _db.Activities.FindAsync(id);
            if (act == null)
                throw new Exception("Активність не знайдено.");

            // Якщо є StudentActivities
            bool hasRefs = await _db.StudentActivities
                .AnyAsync(sa => sa.ActivityId == id);
            if (hasRefs)
                throw new Exception("Не можна видалити активність — є записи у студентів.");

            _db.Activities.Remove(act);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
