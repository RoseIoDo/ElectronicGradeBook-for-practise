using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ElectronicGradeBook.Data;
using ElectronicGradeBook.Models;
using ElectronicGradeBook.Models.Entities.Core;
using ElectronicGradeBook.Models.ViewModels;
using ElectronicGradeBook.Services.Interfaces;
using ElectronicGradeBook.Models.Filters;

namespace ElectronicGradeBook.Controllers
{
    [Route("[controller]/[action]")]
    public class TeachersController : Controller
    {
        private readonly ITeacherService _service;
        public TeachersController(ITeacherService service)
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

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] TeacherViewModel model)
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
        public async Task<IActionResult> Edit([FromBody] TeacherViewModel model)
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
                return Json(new { success = true, message = "Викладача видалено." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetFiltered([FromQuery] TeacherFilterModel filter)
        {
            try
            {
                var paged = await _service.GetFilteredAsync(filter);
                return Json(new { success = true, data = paged });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


    }
}
