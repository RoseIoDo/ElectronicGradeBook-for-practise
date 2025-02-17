using Microsoft.AspNetCore.Mvc;
using ElectronicGradeBook.Services.Interfaces;
using ElectronicGradeBook.Models.ViewModels;
using ElectronicGradeBook.Models.Filters;

namespace ElectronicGradeBook.Controllers
{
    [Route("[controller]/[action]")]
    public class StudyProgramsController : Controller
    {
        private readonly IStudyProgramService _studyProgramService;

        public StudyProgramsController(IStudyProgramService studyProgramService)
        {
            _studyProgramService = studyProgramService;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var programs = await _studyProgramService.GetAllAsync();
            return Json(programs);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] StudyProgramViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Некоректні дані." });
            }
            try
            {
                var createdProgram = await _studyProgramService.CreateAsync(model);
                return Json(new { success = true, data = createdProgram });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] StudyProgramViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Некоректні дані." });
            }
            try
            {
                var updatedProgram = await _studyProgramService.UpdateAsync(model);
                return Json(new { success = true, data = updatedProgram });
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
                await _studyProgramService.DeleteAsync(id);
                return Json(new { success = true, message = "Освітню програму видалено." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetFiltered([FromQuery] StudyProgramFilterModel filter)
        {
            try
            {
                var filteredPrograms = await _studyProgramService.GetFilteredAsync(filter);
                return Json(new { success = true, data = filteredPrograms });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
