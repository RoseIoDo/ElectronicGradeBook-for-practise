using Microsoft.AspNetCore.Mvc;
using ElectronicGradeBook.Services.Interfaces;
using ElectronicGradeBook.Models.ViewModels;
using ElectronicGradeBook.Models.Filters;

namespace ElectronicGradeBook.Controllers
{
    [Route("[controller]/[action]")]
    public class SpecialtiesController : Controller
    {
        private readonly ISpecialtyService _specialtyService;
        private readonly IFacultyService _facultyService;
        private readonly IStudyProgramService _studyProgramService;

        public SpecialtiesController(
            ISpecialtyService specialtyService,
            IFacultyService facultyService,
            IStudyProgramService studyProgramService)
        {
            _specialtyService = specialtyService;
            _facultyService = facultyService;
            _studyProgramService = studyProgramService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Завантажуємо дані для випадаючих списків
            ViewBag.Faculties = await _facultyService.GetAllAsync();
            ViewBag.Programs = await _studyProgramService.GetAllAsync();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _specialtyService.GetAllAsync();
            return Json(list);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] SpecialtyViewModel model)
        {
            try
            {
                var created = await _specialtyService.CreateAsync(model);
                return Json(new { success = true, data = created });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] SpecialtyViewModel model)
        {
            try
            {
                var updated = await _specialtyService.UpdateAsync(model);
                return Json(new { success = true, data = updated });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _specialtyService.DeleteAsync(id);
                return Json(new { success = true, message = "Спеціальність видалено." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetFiltered([FromQuery] SpecialtyFilterModel filter)
        {
            try
            {
                var paged = await _specialtyService.GetFilteredAsync(filter);
                return Json(new { success = true, data = paged });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
