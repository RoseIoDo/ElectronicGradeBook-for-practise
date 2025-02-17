using ElectronicGradeBook.Data;
using ElectronicGradeBook.Models.Entities.Core;
using ElectronicGradeBook.Models.ViewModels;
using ElectronicGradeBook.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ElectronicGradeBook.Services.Implementations
{
    public class GradeBookService : IGradeBookService
    {
        private readonly ApplicationDbContext _db;
        public GradeBookService(ApplicationDbContext db)
        {
            _db = db;
        }

        // ----------- ІСТОРІЯ ОЦІНОК (повний масив) -----------
        public async Task<List<GradeVersionItem>> GetGradeHistoryAsync(int studentId, int offeringId)
        {
            var grade = await _db.Grades
                .FirstOrDefaultAsync(g => g.StudentId == studentId && g.SubjectOfferingId == offeringId);

            if (grade == null) return new List<GradeVersionItem>();

            return ParseGradeVersions(grade.GradeVersionJson);
        }

        // ----------- ДОПОМІЖНІ МЕТОДИ -----------
        private List<GradeVersionItem> ParseGradeVersions(string json)
        {
            if (string.IsNullOrEmpty(json)) return new List<GradeVersionItem>();
            try
            {
                return JsonSerializer.Deserialize<List<GradeVersionItem>>(json)
                    ?? new List<GradeVersionItem>();
            }
            catch
            {
                // Якщо парсинг невдалий – повертаємо пустий список
                return new List<GradeVersionItem>();
            }
        }

        // 1) Факультети
        public async Task<List<FacultyViewModel>> GetFacultiesAsync()
        {
            return await _db.Faculties
                .OrderBy(f => f.Name)
                .Select(f => new FacultyViewModel
                {
                    Id = f.Id,
                    Name = f.Name
                })
                .ToListAsync();
        }

        // 2) Спеціальності певного факультету
        public async Task<List<SpecialtyViewModel>> GetSpecialtiesAsync(int facultyId)
        {
            return await _db.Specialties
                .Where(s => s.FacultyId == facultyId)
                .OrderBy(s => s.Name)
                .Select(s => new SpecialtyViewModel
                {
                    Id = s.Id,
                    Name = s.Name
                })
                .ToListAsync();
        }

        // 3) Групи певної спеціальності
        public async Task<List<GroupViewModel>> GetGroupsAsync(int specialtyId)
        {
            return await _db.Groups
                .Where(g => g.SpecialtyId == specialtyId)
                .OrderBy(g => g.GroupPrefix).ThenBy(g => g.GroupNumber)
                .Select(g => new GroupViewModel
                {
                    Id = g.Id,
                    GroupPrefix = g.GroupPrefix,
                    GroupNumber = g.GroupNumber,
                    CurrentStudyYear = g.CurrentStudyYear
                })
                .ToListAsync();
        }

        // 4) SubjectOfferings для "groupVal"
        public async Task<List<SubjectOfferingViewModel>> GetSubjectOfferingsAsync(string groupVal)
        {
            var groupIds = new List<int>();

            if (groupVal.StartsWith("course-"))
            {
                var parts = groupVal.Replace("course-", "").Split('-');
                if (parts.Length < 2)
                    throw new System.Exception("Невірний формат (course-).");
                string prefix = parts[0];
                int studyY = int.Parse(parts[1]);

                var found = await _db.Groups
                    .Where(g => g.GroupPrefix == prefix && g.CurrentStudyYear == studyY)
                    .ToListAsync();
                groupIds.AddRange(found.Select(x => x.Id));
            }
            else
            {
                int singleId = int.Parse(groupVal);
                groupIds.Add(singleId);
            }

            var offQuery = _db.SubjectOfferings
                .Include(o => o.Subject)
                .Include(o => o.Teacher)
                .Include(o => o.SubjectOfferingGroups).ThenInclude(sog => sog.Group)
                .Where(o => o.SubjectOfferingGroups.Any(sog => groupIds.Contains(sog.GroupId)));

            var list = await offQuery.ToListAsync();

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
            }).ToList();

            return result;
        }

        // 5) Завантажити студентів та оцінки
        public async Task<List<StudentGradeDto>> GetGradesForOfferingAsync(int offId)
        {
            var groupIds = await _db.SubjectOfferingGroups
                .Where(x => x.SubjectOfferingId == offId)
                .Select(x => x.GroupId)
                .ToListAsync();

            var students = await _db.Students
                .Where(s => groupIds.Contains(s.GroupId))
                .Select(s => new { s.Id, s.FullName })
                .OrderBy(s => s.FullName)
                .ToListAsync();

            var gradesRaw = await _db.Grades
                .Where(g => g.SubjectOfferingId == offId)
                .ToListAsync();

            var result = new List<StudentGradeDto>();
            foreach (var st in students)
            {
                var gradeEntity = gradesRaw.FirstOrDefault(x => x.StudentId == st.Id);

                decimal? lastPoints = null;
                bool isRetake = false;
                if (gradeEntity != null && !string.IsNullOrEmpty(gradeEntity.GradeVersionJson))
                {
                    var versions = ParseGradeVersions(gradeEntity.GradeVersionJson);
                    if (versions.Any())
                    {
                        var lastVer = versions.Last();
                        lastPoints = lastVer.Points;
                        isRetake = lastVer.IsRetake;
                    }
                }

                result.Add(new StudentGradeDto
                {
                    StudentId = st.Id,
                    StudentName = st.FullName,
                    Points = lastPoints,
                    IsRetake = isRetake
                });
            }

            return result;
        }

        // 6) Оновити оцінку
        public async Task UpdateGradeAsync(int studentId, int subjectOfferingId, decimal? points, bool isRetake)
        {
            // Перевірка наявності студента
            var student = await _db.Students.FirstOrDefaultAsync(s => s.Id == studentId);
            if (student == null)
                throw new Exception($"Студент з ідентифікатором {studentId} не знайдений.");

            // Перевірка наявності запису SubjectOffering
            var subjectOffering = await _db.SubjectOfferings.FirstOrDefaultAsync(o => o.Id == subjectOfferingId);
            if (subjectOffering == null)
                throw new Exception($"Запис SubjectOffering з ідентифікатором {subjectOfferingId} не знайдений.");

            // Отримуємо існуючий запис оцінки для заданої комбінації
            var grade = await _db.Grades
                .FirstOrDefaultAsync(g => g.StudentId == studentId && g.SubjectOfferingId == subjectOfferingId);

            if (grade == null)
            {
                // Якщо запис не знайдено, створюємо новий і встановлюємо всі обов’язкові поля
                grade = new Grade
                {
                    StudentId = studentId,
                    SubjectOfferingId = subjectOfferingId,
                    DateUpdated = DateTime.Now,
                    IsRetake = isRetake,
                    GradeVersionJson = "[]",
                    Status = "" // Встановлено порожній рядок як значення за замовчуванням, щоб не було NULL
                };
                _db.Grades.Add(grade);
            }

            // Отримуємо існуючі версії оцінок з JSON
            var versions = ParseGradeVersions(grade.GradeVersionJson);

            // Створюємо нову версію оцінки
            var newVer = new GradeVersionItem
            {
                Points = points,
                IsRetake = isRetake,
                Timestamp = DateTime.Now
            };
            versions.Add(newVer);

            // Оновлюємо дані запису
            grade.GradeVersionJson = JsonSerializer.Serialize(versions);
            grade.IsRetake = isRetake;
            grade.DateUpdated = DateTime.Now;

            try
            {
                // Зберігаємо зміни – якщо виникне помилка, inner exception допоможе зрозуміти причину
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Помилка збереження змін: " + ex.InnerException?.Message, ex);
            }
        }

    }
}
