using ElectronicGradeBook.Data;
using ElectronicGradeBook.Models.Entities.Core;
using ElectronicGradeBook.Models.ViewModels;
using ElectronicGradeBook.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ElectronicGradeBook.Services.Implementations
{
    public class GradeService : IGradeService
    {
        private readonly ApplicationDbContext _db;
        public GradeService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<GradeViewModel>> GetAllAsync()
        {
            return await _db.Grades
                .Include(g => g.SubjectOffering).ThenInclude(o => o.Subject)
                .Include(g => g.Student)
                .OrderBy(g => g.DateUpdated)
                .Select(g => new GradeViewModel
                {
                    Id = g.Id,
                    SubjectOfferingId = g.SubjectOfferingId,
                    SubjectName = g.SubjectOffering.Subject.FullName,
                    StudentId = g.StudentId,
                    StudentName = g.Student.FullName,
                    GradeVersionJson = g.GradeVersionJson,
                    Status = g.Status,
                    IsRetake = g.IsRetake,
                    DateUpdated = g.DateUpdated
                })
                .ToListAsync();
        }

        public async Task<GradeViewModel> CreateAsync(GradeViewModel model)
        {
            // Можемо розпарсити JSON, наприклад, дістати Points
            var entity = new Grade
            {
                SubjectOfferingId = model.SubjectOfferingId,
                StudentId = model.StudentId,
                GradeVersionJson = model.GradeVersionJson, // Зберігаємо як є, або формуємо
                Status = model.Status,
                IsRetake = model.IsRetake,
                DateUpdated = DateTime.Now
            };

            // Перевірка, чи не існує вже оцінка для (Student, SubjectOffering)
            bool existGrade = await _db.Grades
                .AnyAsync(g => g.StudentId == model.StudentId
                               && g.SubjectOfferingId == model.SubjectOfferingId);
            if (existGrade)
            {
                // Можна або дозволити дублікати (декілька Attempt?), 
                // або кинути помилку:
                throw new Exception("Оцінка для цього студента і предмета вже існує. " +
                                    "Якщо треба повторне складання, використайте IsRetake.");
            }

            _db.Grades.Add(entity);
            await _db.SaveChangesAsync();

            model.Id = entity.Id;
            model.DateUpdated = entity.DateUpdated;
            return model;
        }

        public async Task<GradeViewModel> UpdateAsync(GradeViewModel model)
        {
            var gr = await _db.Grades.FindAsync(model.Id);
            if (gr == null)
                throw new Exception("Оцінку не знайдено.");

            // Також можна перевірити, чи не міняємо StudentId, SubjectOfferingId
            gr.SubjectOfferingId = model.SubjectOfferingId;
            gr.StudentId = model.StudentId;
            gr.GradeVersionJson = model.GradeVersionJson;
            gr.Status = model.Status;
            gr.IsRetake = model.IsRetake;
            gr.DateUpdated = DateTime.Now;

            await _db.SaveChangesAsync();
            model.DateUpdated = gr.DateUpdated;
            return model;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var gr = await _db.Grades.FindAsync(id);
            if (gr == null)
                throw new Exception("Не знайдено оцінку.");

            _db.Grades.Remove(gr);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
