using ElectronicGradeBook.Data;
using ElectronicGradeBook.Models.Entities.ActivityPrivilege;
using ElectronicGradeBook.Models.ViewModels;
using ElectronicGradeBook.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ElectronicGradeBook.Services.Implementations
{
    public class StudentPrivilegeService : IStudentPrivilegeService
    {
        private readonly ApplicationDbContext _db;
        public StudentPrivilegeService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<StudentPrivilegeViewModel>> GetAllAsync()
        {
            return await _db.StudentPrivileges
                .Include(sp => sp.Student)
                .Include(sp => sp.Privilege)
                .OrderBy(sp => sp.DateGranted)
                .Select(sp => new StudentPrivilegeViewModel
                {
                    Id = sp.Id,
                    StudentId = sp.StudentId,
                    StudentName = sp.Student.FullName,
                    PrivilegeId = sp.PrivilegeId,
                    PrivilegeName = sp.Privilege.Name,
                    DateGranted = sp.DateGranted,
                    DateRevoked = sp.DateRevoked
                })
                .ToListAsync();
        }

        public async Task<StudentPrivilegeViewModel> CreateAsync(StudentPrivilegeViewModel model)
        {
            // Перевірка
            bool stExist = await _db.Students.AnyAsync(s => s.Id == model.StudentId);
            if (!stExist)
                throw new Exception("Студент не знайдено.");

            bool prExist = await _db.Privileges.AnyAsync(p => p.Id == model.PrivilegeId);
            if (!prExist)
                throw new Exception("Пільгу/Привілей не знайдено.");

            var entity = new StudentPrivilege
            {
                StudentId = model.StudentId,
                PrivilegeId = model.PrivilegeId,
                DateGranted = model.DateGranted,
                DateRevoked = model.DateRevoked
            };
            _db.StudentPrivileges.Add(entity);
            await _db.SaveChangesAsync();

            model.Id = entity.Id;
            return model;
        }

        public async Task<StudentPrivilegeViewModel> UpdateAsync(StudentPrivilegeViewModel model)
        {
            var sp = await _db.StudentPrivileges.FindAsync(model.Id);
            if (sp == null)
                throw new Exception("Не знайдено пільгу студента.");

            // Перевірка, чи студент і пільга існують
            sp.StudentId = model.StudentId;
            sp.PrivilegeId = model.PrivilegeId;
            sp.DateGranted = model.DateGranted;
            sp.DateRevoked = model.DateRevoked;

            await _db.SaveChangesAsync();
            return model;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sp = await _db.StudentPrivileges.FindAsync(id);
            if (sp == null)
                throw new Exception("Не знайдено запис пільги студента.");

            _db.StudentPrivileges.Remove(sp);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
