using ElectronicGradeBook.Services.Interfaces;
using ElectronicGradeBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using ElectronicGradeBook.Models.Filters;

namespace ElectronicGradeBook.Controllers
{
    [Route("[controller]/[action]")]
    public class GroupsController : Controller
    {
        private readonly IGroupService _groupService;
        private readonly ISpecialtyService _specialtyService;

        public GroupsController(IGroupService groupService, ISpecialtyService specialtyService)
        {
            _groupService = groupService;
            _specialtyService = specialtyService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Specialties = await _specialtyService.GetAllAsync();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _groupService.GetAllAsync();
            return Json(list);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] GroupViewModel model)
        {
            try
            {
                var created = await _groupService.CreateAsync(model);
                return Json(new { success = true, data = created });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] GroupViewModel model)
        {
            try
            {
                var updated = await _groupService.UpdateAsync(model);
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
                await _groupService.DeleteAsync(id);
                return Json(new { success = true, message = "Групу видалено." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetFiltered([FromQuery] GroupFilterModel filter)
        {
            try
            {
                var paged = await _groupService.GetFilteredAsync(filter);
                return Json(new { success = true, data = paged });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateGroupsAcademicDate(DateTime? customDate)
        {
            // Якщо дата не вказана, використовуємо поточну
            DateTime currentDate = customDate ?? DateTime.Now;
            try
            {
                await _groupService.UpdateGroupsForAcademicDateAsync(currentDate);
                return Json(new { success = true, message = $"Групи оновлено для дати {currentDate:dd.MM.yyyy}" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}
