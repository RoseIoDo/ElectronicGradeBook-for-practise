using ElectronicGradeBook.Services.Interfaces;
using ElectronicGradeBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ElectronicGradeBook.Controllers
{
    [Route("[controller]/[action]")]
    public class SubjectSubgroupsController : Controller
    {
        private readonly ISubjectSubgroupService _service;

        public SubjectSubgroupsController(ISubjectSubgroupService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetByOffering(int offeringId)
        {
            var list = await _service.GetByOfferingAsync(offeringId);
            return Json(list);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] SubjectSubgroupViewModel model)
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
        public async Task<IActionResult> Edit([FromBody] SubjectSubgroupViewModel model)
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
                return Json(new { success = true, message = "Підгрупу видалено." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddStudent(int subgroupId, int studentId)
        {
            try
            {
                await _service.AddStudentToSubgroup(subgroupId, studentId);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveStudent(int subgroupId, int studentId)
        {
            try
            {
                await _service.RemoveStudentFromSubgroup(subgroupId, studentId);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
