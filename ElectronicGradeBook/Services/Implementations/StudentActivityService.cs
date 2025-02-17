using ElectronicGradeBook.Data;
using ElectronicGradeBook.Models.Entities.ActivityPrivilege;
using ElectronicGradeBook.Models.ViewModels;
using ElectronicGradeBook.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ElectronicGradeBook.Services.Implementations
{
    public class StudentActivityService : IStudentActivityService
    {
        private readonly ApplicationDbContext _db;
        public StudentActivityService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<StudentActivityViewModel>> GetAllAsync()
        {
            return await _db.StudentActivities
                .Include(sa => sa.Student)
                .Include(sa => sa.Activity)
                .OrderBy(sa => sa.DateAwarded)
                .Select(sa => new StudentActivityViewModel
                {
                    Id = sa.Id,
                    StudentId = sa.StudentId,
                    StudentName = sa.Student.FullName,
                    ActivityId = sa.ActivityId,
                    ActivityName = sa.Activity.Name,
                    DateAwarded = sa.DateAwarded,
                    Semester = sa.Semester,
                    Notes = sa.Notes
                })
                .ToListAsync();
        }

        public async Task<StudentActivityViewModel> CreateAsync(StudentActivityViewModel model)
        {
            // Перевірка, наприклад, щоб ActivityId і StudentId існували
            var studentExist = await _db.Students.AnyAsync(s => s.Id == model.StudentId);
            if (!studentExist)
                throw new Exception("Студент не існує.");

            var activityExist = await _db.Activities.AnyAsync(a => a.Id == model.ActivityId);
            if (!activityExist)
                throw new Exception("Активність не існує.");

            var entity = new StudentActivity
            {
                StudentId = model.StudentId,
                ActivityId = model.ActivityId,
                DateAwarded = model.DateAwarded,
                Semester = model.Semester,
                Notes = model.Notes
            };
            _db.StudentActivities.Add(entity);
            await _db.SaveChangesAsync();

            model.Id = entity.Id;
            return model;
        }

        public async Task<StudentActivityViewModel> UpdateAsync(StudentActivityViewModel model)
        {
            var sa = await _db.StudentActivities.FindAsync(model.Id);
            if (sa == null)
                throw new Exception("Студентську активність не знайдено.");

            sa.StudentId = model.StudentId;
            sa.ActivityId = model.ActivityId;
            sa.DateAwarded = model.DateAwarded;
            sa.Semester = model.Semester;
            sa.Notes = model.Notes;

            await _db.SaveChangesAsync();
            return model;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sa = await _db.StudentActivities.FindAsync(id);
            if (sa == null)
                throw new Exception("Студентську активність не знайдено.");

            _db.StudentActivities.Remove(sa);
            await _db.SaveChangesAsync();
            return true;
        }
    }

}
