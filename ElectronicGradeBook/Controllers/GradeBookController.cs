using ElectronicGradeBook.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ElectronicGradeBook.Controllers
{
    [Route("[controller]/[action]")]
    public class GradeBookController : Controller
    {
        private readonly IGradeBookService _gradeBookService;

        public GradeBookController(IGradeBookService gradeBookService)
        {
            _gradeBookService = gradeBookService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(); // Views/GradeBook/Index.cshtml
        }

        // ---------- 1) Для завантаження списку факультетів ----------
        [HttpGet]
        public async Task<IActionResult> GetFaculties()
        {
            var list = await _gradeBookService.GetFacultiesAsync();
            return Json(list);
        }

        // ---------- 2) Для завантаження спеціальностей певного факультету ----------
        [HttpGet]
        public async Task<IActionResult> GetSpecialties(int facultyId)
        {
            var specs = await _gradeBookService.GetSpecialtiesAsync(facultyId);
            return Json(specs);
        }

        // ---------- 3) Для завантаження груп, вказавши specialtyId ----------
        [HttpGet]
        public async Task<IActionResult> GetGroups(int specialtyId)
        {
            var groups = await _gradeBookService.GetGroupsAsync(specialtyId);
            return Json(groups);
        }

        // ---------- 4) Завантажити SubjectOfferings для обраної групи/курсу ----------
        [HttpGet]
        public async Task<IActionResult> GetSubjectOfferings(string groupVal)
        {
            // groupVal може бути формату "course-KN-1" або просто "5"
            var offerings = await _gradeBookService.GetSubjectOfferingsAsync(groupVal);
            return Json(offerings);
        }

        // ---------- 5) Завантажити оцінки (останню версію) для вказаного Offering ----------
        [HttpGet]
        public async Task<IActionResult> GetGradesForOffering(int offId)
        {
            var rows = await _gradeBookService.GetGradesForOfferingAsync(offId);
            return Json(rows);
        }

        // ---------- 6) Оновити оцінку (додати нову версію) ----------
        [HttpPost]
        public async Task<IActionResult> UpdateGrade([FromBody] UpdateGradeModel model)
        {
            if (model == null)
                return Json(new { success = false, message = "No data" });

            try
            {
                await _gradeBookService.UpdateGradeAsync(
                    model.StudentId,
                    model.SubjectOfferingId,
                    model.Points,
                    model.IsRetake
                );
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ---------- 7) Показати ІСТОРІЮ версій оцінок (для 1 студента) ----------
        [HttpGet]
        public async Task<IActionResult> GetGradeHistory(int studentId, int offId)
        {
            var history = await _gradeBookService.GetGradeHistoryAsync(studentId, offId);
            return Json(history);
        }
    }

    // Модель для оновлення оцінки
    public class UpdateGradeModel
    {
        public int StudentId { get; set; }
        public int SubjectOfferingId { get; set; }
        public decimal? Points { get; set; }
        public bool IsRetake { get; set; }
    }
}
