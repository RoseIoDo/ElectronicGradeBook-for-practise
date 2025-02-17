using ElectronicGradeBook.Data;
using ElectronicGradeBook.Models.Entities.Core;
using ElectronicGradeBook.Models.ViewModels;
using ElectronicGradeBook.Models.Filters;
using ElectronicGradeBook.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ElectronicGradeBook.Services.Implementations
{
    public class SubjectOfferingService : ISubjectOfferingService
    {
        private readonly ApplicationDbContext _db;

        public SubjectOfferingService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<SubjectOfferingViewModel>> GetAllAsync()
        {
            var list = await _db.SubjectOfferings
                .Include(o => o.Subject)
                .Include(o => o.Teacher)
                .Include(o => o.SubjectOfferingGroups)
                    .ThenInclude(sog => sog.Group)
                .OrderBy(o => o.Id)
                .ToListAsync();

            // Мапимо до ViewModel
            var result = list.Select(off => new SubjectOfferingViewModel
            {
                Id = off.Id,
                SubjectId = off.SubjectId,
                SubjectName = off.Subject.FullName,
                TeacherId = off.TeacherId,
                TeacherName = off.Teacher.FullName,

                YearOfStudy = off.YearOfStudy,
                SemesterInYear = off.SemesterInYear,
                Credits = off.Credits,

                IsSubjectElective = off.Subject.IsElective,
                CombinedGroupNames = off.SubjectOfferingGroups
                    .Select(sog => sog.Group.GroupPrefix + "-" + sog.Group.GroupNumber)
                    .ToList()
            })
            .ToList();

            return result;
        }


        public async Task<PagedResult<SubjectOfferingViewModel>> GetFilteredAsync(SubjectOfferingFilterModel filter)
        {
            var query = _db.SubjectOfferings
                .Include(o => o.Subject)
                .Include(o => o.Teacher)
                .Include(o => o.SubjectOfferingGroups).ThenInclude(sog => sog.Group)
                .AsQueryable();

            // Якщо треба фільтрувати за SubjectId
            if (filter.SubjectId.HasValue && filter.SubjectId.Value > 0)
            {
                query = query.Where(o => o.SubjectId == filter.SubjectId.Value);
            }

            // Якщо треба фільтрувати за TeacherId
            if (filter.TeacherId.HasValue && filter.TeacherId.Value > 0)
            {
                query = query.Where(o => o.TeacherId == filter.TeacherId.Value);
            }

            // Якщо треба фільтрувати за GroupId (шукаємо в bridging)
            if (filter.GroupId.HasValue && filter.GroupId.Value > 0)
            {
                query = query.Where(o =>
                    o.SubjectOfferingGroups.Any(sog => sog.GroupId == filter.GroupId.Value));
            }

            // Семестр
            if (filter.SemesterInYear.HasValue)
            {
                query = query.Where(o => o.SemesterInYear == filter.SemesterInYear.Value);
            }

            int total = await query.CountAsync();
            int skip = (filter.PageNumber - 1) * filter.PageSize;

            var items = await query
                .OrderBy(o => o.AcademicYear).ThenBy(o => o.Id)
                .Skip(skip).Take(filter.PageSize)
                .ToListAsync();

            // Перетворюємо на ViewModel
            var vmList = items.Select(off => new SubjectOfferingViewModel
            {
                Id = off.Id,
                SubjectId = off.SubjectId,
                SubjectName = off.Subject.FullName,
                TeacherId = off.TeacherId,
                TeacherName = off.Teacher.FullName,

                YearOfStudy = off.YearOfStudy,
                SemesterInYear = off.SemesterInYear,
                AcademicYear = off.AcademicYear,
                Credits = off.Credits,

                CombinedGroupNames = off.SubjectOfferingGroups
                    .Select(sog => sog.Group.GroupPrefix + "-" + sog.Group.GroupNumber)
                    .ToList()
            }).ToList();

            return new PagedResult<SubjectOfferingViewModel>
            {
                Items = vmList,
                TotalItems = total,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<SubjectOfferingViewModel> CreateAsync(SubjectOfferingViewModel model)
        {
            // 1) Якщо не задано AcademicYear, то за замовчуванням
            if (string.IsNullOrEmpty(model.AcademicYear))
            {
                int currentYear = (DateTime.Now.Month >= 9) ? DateTime.Now.Year : (DateTime.Now.Year - 1);
                model.AcademicYear = $"{currentYear}-{currentYear + 1}";
            }

            // 2) Створюємо новий SubjectOffering
            var off = new SubjectOffering
            {
                SubjectId = model.SubjectId,
                TeacherId = model.TeacherId,
                YearOfStudy = model.YearOfStudy,
                SemesterInYear = model.SemesterInYear,
                AcademicYear = model.AcademicYear,
                Credits = model.Credits
            };
            _db.SubjectOfferings.Add(off);

            // 3) Якщо вказав “course-KN-1”
            if (!string.IsNullOrEmpty(model.GroupNameOrCourse)
                && model.GroupNameOrCourse.StartsWith("course-"))
            {
                string course = model.GroupNameOrCourse.Replace("course-", "");
                var parts = course.Split('-');
                if (parts.Length < 2)
                    throw new Exception("Невірний формат (course-...).");

                string prefix = parts[0];
                int studyYear = int.Parse(parts[1]);

                // Знайти всі групи
                var groups = await _db.Groups
                    .Where(g => g.GroupPrefix == prefix && g.CurrentStudyYear == studyYear)
                    .ToListAsync();

                if (!groups.Any())
                    throw new Exception($"Не знайдено груп для '{course}'.");

                // Додаємо SubjectOfferingGroup для кожної знайденої групи
                foreach (var g in groups)
                {
                    _db.SubjectOfferingGroups.Add(new SubjectOfferingGroup
                    {
                        SubjectOffering = off,
                        GroupId = g.Id
                    });
                }
            }
            else if (model.SingleGroupId.HasValue && model.SingleGroupId.Value > 0)
            {
                // Якщо користувач просто обрав одну групу
                _db.SubjectOfferingGroups.Add(new SubjectOfferingGroup
                {
                    SubjectOffering = off,
                    GroupId = model.SingleGroupId.Value
                });
            }
            else
            {
                // Можливо, предмет зовсім без офіційних груп (наприклад, вибірковий, 
                // де групи визначатимуться через підгрупи).
                // => нічого не робимо
            }

            await _db.SaveChangesAsync();
            model.Id = off.Id;

            // 4) Для відображення CombinedGroupNames
            model.CombinedGroupNames = await _db.SubjectOfferingGroups
                .Where(sog => sog.SubjectOfferingId == off.Id)
                .Include(sog => sog.Group)
                .Select(sog => sog.Group.GroupPrefix + "-" + sog.Group.GroupNumber)
                .ToListAsync();

            return model;
        }

        public async Task<SubjectOfferingViewModel> UpdateAsync(SubjectOfferingViewModel model)
        {
            var off = await _db.SubjectOfferings
                .Include(o => o.SubjectOfferingGroups)
                .FirstOrDefaultAsync(o => o.Id == model.Id);

            if (off == null)
                throw new Exception("SubjectOffering не знайдено.");

            off.SubjectId = model.SubjectId;
            off.TeacherId = model.TeacherId;
            off.YearOfStudy = model.YearOfStudy;
            off.SemesterInYear = model.SemesterInYear;
            if (!string.IsNullOrEmpty(model.AcademicYear))
                off.AcademicYear = model.AcademicYear;
            off.Credits = model.Credits;

            // Якщо треба змінювати прив’язані групи, треба видаляти/додавати
            // SubjectOfferingGroups. Тут або лишаємо як є, або прописуємо логіку.
            //  (для спрощення, нехай воно поки не змінюється автоматично).

            await _db.SaveChangesAsync();

            // Повторно завантажимо CombinedGroupNames
            model.CombinedGroupNames = await _db.SubjectOfferingGroups
                .Where(x => x.SubjectOfferingId == off.Id)
                .Include(x => x.Group)
                .Select(x => x.Group.GroupPrefix + "-" + x.Group.GroupNumber)
                .ToListAsync();
            return model;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var off = await _db.SubjectOfferings.FindAsync(id);
            if (off == null)
                throw new Exception("SubjectOffering не знайдено.");

            // Перевірити, чи є оцінки
            bool hasGrades = await _db.Grades.AnyAsync(g => g.SubjectOfferingId == id);
            if (hasGrades)
                throw new Exception("Не можна видалити – є оцінки.");

            _db.SubjectOfferings.Remove(off);
            await _db.SaveChangesAsync();
            return true;
        }


    }
}
