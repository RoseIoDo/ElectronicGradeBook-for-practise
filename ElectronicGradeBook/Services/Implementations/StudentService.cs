using ElectronicGradeBook.Data;
using ElectronicGradeBook.Models.Entities.Core;
using ElectronicGradeBook.Models.Filters;
using ElectronicGradeBook.Models.ViewModels;
using ElectronicGradeBook.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ElectronicGradeBook.Services.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly ApplicationDbContext _db;
        public StudentService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<StudentViewModel>> GetAllAsync()
        {
            return await _db.Students
                .Include(s => s.Group)
                    .ThenInclude(g => g.Specialty)
                .OrderBy(s => s.FullName)
                .Select(s => new StudentViewModel
                {
                    Id = s.Id,
                    FullName = s.FullName,
                    EnrollmentDate = s.EnrollmentDate,
                    GraduationDate = s.GraduationDate,
                    IsActive = s.IsActive,
                    GroupId = s.GroupId,
                    GroupName = s.Group.GroupPrefix + "-" + s.Group.GroupNumber,
                    GroupPrefix = s.Group.GroupPrefix,
                    GroupNumber = s.Group.GroupNumber,
                    GroupEnrollmentYear = s.Group.EnrollmentYear,
                    GroupGraduationYear = s.Group.GraduationYear,
                    GroupCurrentStudyYear = s.Group.CurrentStudyYear,
                    SpecialtyName = s.Group.Specialty.Name
                })
                .ToListAsync();
        }

        public async Task<PagedResult<StudentViewModel>> GetFilteredAsync(StudentFilterModel filter)
        {
            var query = _db.Students
                .Include(s => s.Group)
                    .ThenInclude(g => g.Specialty)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter.Search))
            {
                query = query.Where(s => s.FullName.Contains(filter.Search));
            }
            if (filter.GroupId.HasValue && filter.GroupId.Value > 0)
            {
                query = query.Where(s => s.GroupId == filter.GroupId.Value);
            }
            if (filter.IsActive.HasValue)
            {
                query = query.Where(s => s.IsActive == filter.IsActive.Value);
            }

            int totalItems = await query.CountAsync();
            int skip = (filter.PageNumber - 1) * filter.PageSize;

            var items = await query
                .OrderBy(s => s.FullName)
                .Skip(skip)
                .Take(filter.PageSize)
                .Select(s => new StudentViewModel
                {
                    Id = s.Id,
                    FullName = s.FullName,
                    EnrollmentDate = s.EnrollmentDate,
                    GraduationDate = s.GraduationDate,
                    IsActive = s.IsActive,
                    GroupId = s.GroupId,
                    GroupName = s.Group.GroupPrefix + "-" + s.Group.GroupNumber,
                    GroupPrefix = s.Group.GroupPrefix,
                    GroupNumber = s.Group.GroupNumber,
                    GroupEnrollmentYear = s.Group.EnrollmentYear,
                    GroupGraduationYear = s.Group.GraduationYear,
                    GroupCurrentStudyYear = s.Group.CurrentStudyYear,
                    SpecialtyName = s.Group.Specialty.Name
                })
                .ToListAsync();

            return new PagedResult<StudentViewModel>
            {
                Items = items,
                TotalItems = totalItems,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<StudentViewModel> CreateAsync(StudentViewModel model)
        {
            // перевірка, чи не дублюється ПІБ і рік зарахування
            bool existSame = await _db.Students
                .AnyAsync(x => x.FullName == model.FullName && x.EnrollmentDate == model.EnrollmentDate);
            if (existSame)
                throw new Exception($"Студент {model.FullName} уже існує з такою датою зарахування.");

            var entity = new Student
            {
                FullName = model.FullName,
                EnrollmentDate = model.EnrollmentDate,
                GraduationDate = model.GraduationDate,
                IsActive = model.IsActive,
                GroupId = model.GroupId
            };
            _db.Students.Add(entity);
            await _db.SaveChangesAsync();
            model.Id = entity.Id;
            return model;
        }

        public async Task<StudentViewModel> UpdateAsync(StudentViewModel model)
        {
            var st = await _db.Students.FindAsync(model.Id);
            if (st == null)
                throw new Exception("Студента не знайдено.");

            // Аналогічна перевірка
            bool existSame = await _db.Students
                .AnyAsync(x => x.FullName == model.FullName
                               && x.EnrollmentDate == model.EnrollmentDate
                               && x.Id != model.Id);
            if (existSame)
                throw new Exception($"Студент {model.FullName} уже існує з такою датою зарахування.");

            st.FullName = model.FullName;
            st.EnrollmentDate = model.EnrollmentDate;
            st.GraduationDate = model.GraduationDate;
            st.IsActive = model.IsActive;
            st.GroupId = model.GroupId;

            await _db.SaveChangesAsync();
            return model;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var st = await _db.Students.FindAsync(id);
            if (st == null)
                throw new Exception("Студента не знайдено.");

            /* Перевірка, чи має студент оцінки
            bool hasGrades = await _db.Grades
                .AnyAsync(g => g.StudentId == id);
            if (hasGrades)
                throw new Exception("Не можна видалити студента — є виставлені оцінки.");

            // Перевірка, чи є активності
            bool hasActivities = await _db.StudentActivities
                .AnyAsync(a => a.StudentId == id);
            if (hasActivities)
                throw new Exception("Не можна видалити — є активності студента.");

            // Перевірка, чи є привілеї
            bool hasPrivileges = await _db.StudentPrivileges
                .AnyAsync(p => p.StudentId == id);
            if (hasPrivileges)
                throw new Exception("Не можна видалити — є пільги студента.");*/

            _db.Students.Remove(st);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
