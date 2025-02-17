using ElectronicGradeBook.Data;
using ElectronicGradeBook.Models.Entities.Core;
using ElectronicGradeBook.Models.ViewModels;
using ElectronicGradeBook.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ElectronicGradeBook.Services.Implementations
{
    public class SubjectSubgroupService : ISubjectSubgroupService
    {
        private readonly ApplicationDbContext _db;

        public SubjectSubgroupService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<SubjectSubgroupViewModel>> GetByOfferingAsync(int offeringId)
        {
            return await _db.SubjectSubgroups
                .Include(sg => sg.Teacher)
                .Where(sg => sg.SubjectOfferingId == offeringId)
                .OrderBy(sg => sg.Name)
                .Select(sg => new SubjectSubgroupViewModel
                {
                    Id = sg.Id,
                    SubjectOfferingId = sg.SubjectOfferingId,
                    Name = sg.Name,
                    TeacherId = sg.TeacherId,
                    TeacherName = sg.Teacher != null ? sg.Teacher.FullName : null
                })
                .ToListAsync();
        }

        public async Task<SubjectSubgroupViewModel> CreateAsync(SubjectSubgroupViewModel model)
        {
            // Перевіряємо, що SubjectOffering існує
            var off = await _db.SubjectOfferings.FindAsync(model.SubjectOfferingId);
            if (off == null)
                throw new Exception("SubjectOffering not found.");

            bool existName = await _db.SubjectSubgroups
                .AnyAsync(x => x.SubjectOfferingId == model.SubjectOfferingId && x.Name == model.Name);
            if (existName)
                throw new Exception($"Підгрупа '{model.Name}' вже існує.");

            var entity = new SubjectSubgroup
            {
                SubjectOfferingId = model.SubjectOfferingId,
                Name = model.Name,
                TeacherId = model.TeacherId
            };
            _db.SubjectSubgroups.Add(entity);
            await _db.SaveChangesAsync();

            model.Id = entity.Id;
            return model;
        }

        public async Task<SubjectSubgroupViewModel> UpdateAsync(SubjectSubgroupViewModel model)
        {
            var sg = await _db.SubjectSubgroups.FindAsync(model.Id);
            if (sg == null)
                throw new Exception("Підгрупу не знайдено.");

            bool existSame = await _db.SubjectSubgroups
                .AnyAsync(x => x.SubjectOfferingId == sg.SubjectOfferingId
                               && x.Name == model.Name
                               && x.Id != model.Id);
            if (existSame)
                throw new Exception($"Підгрупа '{model.Name}' вже існує.");

            sg.Name = model.Name;
            sg.TeacherId = model.TeacherId;
            await _db.SaveChangesAsync();
            return model;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sg = await _db.SubjectSubgroups.FindAsync(id);
            if (sg == null)
                throw new Exception("Підгрупу не знайдено.");

            _db.SubjectSubgroups.Remove(sg);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddStudentToSubgroup(int subgroupId, int studentId)
        {
            // Перевіряємо, чи не існує вже
            bool already = await _db.SubjectSubgroupStudents
                .AnyAsync(sss => sss.SubjectSubgroupId == subgroupId && sss.StudentId == studentId);
            if (already)
                throw new Exception("Студент уже в цій підгрупі.");

            var entry = new SubjectSubgroupStudent
            {
                SubjectSubgroupId = subgroupId,
                StudentId = studentId
            };
            _db.SubjectSubgroupStudents.Add(entry);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveStudentFromSubgroup(int subgroupId, int studentId)
        {
            var record = await _db.SubjectSubgroupStudents
                .FirstOrDefaultAsync(x => x.SubjectSubgroupId == subgroupId && x.StudentId == studentId);
            if (record == null)
                throw new Exception("Запис не знайдено.");

            _db.SubjectSubgroupStudents.Remove(record);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
