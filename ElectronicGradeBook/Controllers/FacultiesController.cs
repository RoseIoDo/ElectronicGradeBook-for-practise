using ElectronicGradeBook.Models.ViewModels;
using ElectronicGradeBook.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ElectronicGradeBook.Controllers
{
    [Route("[controller]/[action]")]
    public class FacultiesController : Controller
    {
        private readonly IFacultyService _service;
        public FacultiesController(IFacultyService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Json(list);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] FacultyViewModel model)
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
        public async Task<IActionResult> Edit([FromBody] FacultyViewModel model)
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
                return Json(new { success = true, message = "Факультет видалено." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
