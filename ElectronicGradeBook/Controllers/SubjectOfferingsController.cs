using ElectronicGradeBook.Services.Interfaces;
using ElectronicGradeBook.Models.ViewModels;
using ElectronicGradeBook.Models.Filters;
using Microsoft.AspNetCore.Mvc;

namespace ElectronicGradeBook.Controllers
{
    [Route("[controller]/[action]")]
    public class SubjectOfferingsController : Controller
    {
        private readonly ISubjectOfferingService _service;

        public SubjectOfferingsController(ISubjectOfferingService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Json(list);
        }

        [HttpGet]
        public async Task<IActionResult> GetFiltered([FromQuery] SubjectOfferingFilterModel filter)
        {
            var paged = await _service.GetFilteredAsync(filter);
            return Json(paged);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] SubjectOfferingViewModel model)
        {
            try
            {
                var created = await _service.CreateAsync(model);
                return Json(new { success = true, data = created });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] SubjectOfferingViewModel model)
        {
            try
            {
                var updated = await _service.UpdateAsync(model);
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
                await _service.DeleteAsync(id);
                return Json(new { success = true, message = "Запис вилучено" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
