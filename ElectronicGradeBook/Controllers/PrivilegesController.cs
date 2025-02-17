using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ElectronicGradeBook.Data;
using ElectronicGradeBook.Models;
using ElectronicGradeBook.Models.Entities.ActivityPrivilege;
using ElectronicGradeBook.Models.ViewModels;
using ElectronicGradeBook.Services.Interfaces;

namespace ElectronicGradeBook.Controllers
{
    [Route("[controller]/[action]")]
    public class PrivilegesController : Controller
    {
        private readonly IPrivilegeService _service;
        public PrivilegesController(IPrivilegeService service)
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
        public async Task<IActionResult> Add([FromBody] PrivilegeViewModel model)
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
        public async Task<IActionResult> Edit([FromBody] PrivilegeViewModel model)
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
                return Json(new { success = true, message = "Пільгу/привілей видалено." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
