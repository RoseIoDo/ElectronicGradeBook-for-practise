using ElectronicGradeBook.Data;
using ElectronicGradeBook.Models.Entities.Core;
using ElectronicGradeBook.Models.Filters;
using ElectronicGradeBook.Models.ViewModels;
using ElectronicGradeBook.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectronicGradeBook.Services.Implementations
{
    public class GroupService : IGroupService
    {
        private readonly ApplicationDbContext _db;
        // Максимальна тривалість бакалаврської програми
        private const int DefaultStudyProgramDuration = 4;

        public GroupService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<GroupViewModel>> GetAllAsync()
        {
            // Для обчислення статусу використаємо поточну академічну дату:
            int academicYearStart = DateTime.Now.Month >= 9 ? DateTime.Now.Year : DateTime.Now.Year - 1;

            return await _db.Groups
                .Include(g => g.Specialty)
                .OrderBy(g => g.GroupPrefix)
                .Select(g => new GroupViewModel
                {
                    Id = g.Id,
                    GroupPrefix = g.GroupPrefix,
                    GroupNumber = g.GroupNumber,
                    CurrentStudyYear = g.CurrentStudyYear,
                    EnrollmentYear = g.EnrollmentYear,
                    GraduationYear = g.GraduationYear,
                    SpecialtyId = g.SpecialtyId,
                    SpecialtyName = g.Specialty.Name,
                    // Якщо поточний академічний рік вже досяг або перевищує рік випуску, група вважається випущеною
                    IsGraduated = academicYearStart >= g.GraduationYear
                })
                .ToListAsync();
        }

        public async Task<PagedResult<GroupViewModel>> GetFilteredAsync(GroupFilterModel filter)
        {
            var query = _db.Groups
                .Include(g => g.Specialty)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter.PrefixSearch))
            {
                query = query.Where(g => g.GroupPrefix.Contains(filter.PrefixSearch));
            }

            if (filter.SpecialtyId.HasValue && filter.SpecialtyId.Value > 0)
            {
                query = query.Where(g => g.SpecialtyId == filter.SpecialtyId.Value);
            }

            if (filter.EnrollmentYear.HasValue)
            {
                query = query.Where(g => g.EnrollmentYear == filter.EnrollmentYear.Value);
            }

            int totalItems = await query.CountAsync();
            int skip = (filter.PageNumber - 1) * filter.PageSize;

            int academicYearStart = DateTime.Now.Month >= 9 ? DateTime.Now.Year : DateTime.Now.Year - 1;

            var items = await query
                .OrderBy(g => g.GroupPrefix)
                .Skip(skip)
                .Take(filter.PageSize)
                .Select(g => new GroupViewModel
                {
                    Id = g.Id,
                    GroupPrefix = g.GroupPrefix,
                    GroupNumber = g.GroupNumber,
                    CurrentStudyYear = g.CurrentStudyYear,
                    EnrollmentYear = g.EnrollmentYear,
                    GraduationYear = g.GraduationYear,
                    SpecialtyId = g.SpecialtyId,
                    SpecialtyName = g.Specialty.Name,
                    IsGraduated = academicYearStart >= g.GraduationYear
                })
                .ToListAsync();

            return new PagedResult<GroupViewModel>
            {
                Items = items,
                TotalItems = totalItems,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<GroupViewModel> CreateAsync(GroupViewModel model)
        {
            // Автоматичне обчислення поточного курсу, якщо не задано
            if (model.CurrentStudyYear <= 0)
            {
                int currentAcademicYear = DateTime.Now.Month >= 9 ? DateTime.Now.Year : DateTime.Now.Year - 1;
                model.CurrentStudyYear = currentAcademicYear - model.EnrollmentYear + 1;
            }

            // Якщо рік випуску не задано, встановлюємо його як рік вступу + максимальна тривалість
            if (model.GraduationYear <= 0)
            {
                model.GraduationYear = model.EnrollmentYear + DefaultStudyProgramDuration;
            }

            bool existGroup = await _db.Groups
                .AnyAsync(x => x.GroupPrefix == model.GroupPrefix && x.GroupNumber == model.GroupNumber);
            if (existGroup)
                throw new Exception($"Група '{model.GroupPrefix}-{model.GroupNumber}' вже існує.");

            var entity = new Group
            {
                GroupPrefix = model.GroupPrefix,
                GroupNumber = model.GroupNumber,
                CurrentStudyYear = model.CurrentStudyYear,
                EnrollmentYear = model.EnrollmentYear,
                GraduationYear = model.GraduationYear,
                SpecialtyId = model.SpecialtyId
            };
            _db.Groups.Add(entity);
            await _db.SaveChangesAsync();

            model.Id = entity.Id;
            return model;
        }

        public async Task<GroupViewModel> UpdateAsync(GroupViewModel model)
        {
            var gr = await _db.Groups.FindAsync(model.Id);
            if (gr == null)
                throw new Exception("Групу не знайдено.");

            bool existSame = await _db.Groups
                .AnyAsync(x => x.GroupPrefix == model.GroupPrefix && x.GroupNumber == model.GroupNumber && x.Id != model.Id);
            if (existSame)
                throw new Exception($"Група '{model.GroupPrefix}-{model.GroupNumber}' вже існує.");

            gr.GroupPrefix = model.GroupPrefix;
            gr.GroupNumber = model.GroupNumber;
            gr.CurrentStudyYear = model.CurrentStudyYear;
            gr.EnrollmentYear = model.EnrollmentYear;
            gr.GraduationYear = model.GraduationYear;
            gr.SpecialtyId = model.SpecialtyId;

            await _db.SaveChangesAsync();
            return model;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var gr = await _db.Groups.FindAsync(id);
            if (gr == null)
                throw new Exception("Групу не знайдено.");


            bool hasOfferings = await _db.SubjectOfferingGroups.AnyAsync(sog => sog.GroupId == id);
            if (hasOfferings)
                throw new Exception("Не можна видалити групу — існують SubjectOfferings, що її використовують.");

            // Далі – перевірка, чи є студенти, якщо треба:
            bool hasStudents = await _db.Students.AnyAsync(s => s.GroupId == id);
            if (hasStudents)
                throw new Exception("Не можна видалити групу — є студенти.");

            _db.Groups.Remove(gr);
            await _db.SaveChangesAsync();
            return true;
        }


        /// <summary>
        /// Оновлює поточний курс та рік випуску для всіх груп,
        /// виходячи з переданої дати.
        /// поки що визначається за правилом: 01.09 - 30.06.
        /// </summary>
        public async Task UpdateGroupsForAcademicDateAsync(DateTime currentDate)
        {
            int academicYearStart = currentDate.Month >= 9 ? currentDate.Year : currentDate.Year - 1;
            var groups = await _db.Groups.Include(g => g.Specialty).ToListAsync();

            foreach (var group in groups)
            {
                int newCourse = academicYearStart - group.EnrollmentYear + 1;
                // Якщо новий курс перевищує максимальну тривалість, вважаємо групу випущеною
                group.CurrentStudyYear = newCourse > DefaultStudyProgramDuration ? DefaultStudyProgramDuration : newCourse;
                // Рік випуску встановлюємо як рік вступу + максимальна тривалість
                group.GraduationYear = group.EnrollmentYear + DefaultStudyProgramDuration;
            }
            await _db.SaveChangesAsync();
        }
        public async Task<List<GroupViewModel>> GetGroupsByValAsync(string groupVal)
        {
            // Якщо groupVal починається з “course-…”
            if (groupVal.StartsWith("course-"))
            {
                // парсимо prefix + studyYear
                var pr = groupVal.Replace("course-", "").Split('-');
                if (pr.Length < 2) return new List<GroupViewModel>();
                string prefix = pr[0];
                int cYear = int.Parse(pr[1]);

                var groups = await _db.Groups
                    .Where(g => g.GroupPrefix == prefix && g.CurrentStudyYear == cYear)
                    .Select(g => new GroupViewModel
                    {
                        Id = g.Id,
                        GroupPrefix = g.GroupPrefix,
                        GroupNumber = g.GroupNumber,
                        CurrentStudyYear = g.CurrentStudyYear,
                        EnrollmentYear = g.EnrollmentYear,
                        GraduationYear = g.GraduationYear
                    })
                    .ToListAsync();
                return groups;
            }
            else
            {
                // одинична група
                if (int.TryParse(groupVal, out int gid))
                {
                    var g = await _db.Groups
                        .Where(x => x.Id == gid)
                        .Select(x => new GroupViewModel
                        {
                            Id = x.Id,
                            GroupPrefix = x.GroupPrefix,
                            GroupNumber = x.GroupNumber,
                            CurrentStudyYear = x.CurrentStudyYear,
                            EnrollmentYear = x.EnrollmentYear,
                            GraduationYear = x.GraduationYear
                        })
                        .FirstOrDefaultAsync();
                    if (g != null)
                        return new List<GroupViewModel> { g };
                }
                return new List<GroupViewModel>();
            }
        }

        public async Task<List<int>> ResolveGroupIdsAsync(string groupVal)
        {
            var groups = await GetGroupsByValAsync(groupVal);
            return groups.Select(g => g.Id).ToList();
        }

        // Метод GenerateSubgroupsAsync можливо на майбутнє (нагадувалка)
    }
}
