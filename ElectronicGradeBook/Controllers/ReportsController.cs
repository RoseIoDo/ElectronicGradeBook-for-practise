using ElectronicGradeBook.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ElectronicGradeBook.Controllers
{
    [Route("[controller]/[action]")]
    public class ReportsController : Controller
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Сторінка з кнопками чи посиланнями на звіти
            return View();
        }

        // --------- EXCEL ----------
        [HttpGet]
        public async Task<IActionResult> GradeBookExcel(int groupId, int semester)
        {
            try
            {
                var bytes = await _reportService.GenerateGradeBookExcelAsync(groupId, semester);
                string fileName = $"GradeBook_{groupId}_sem{semester}.xlsx";
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                return File(bytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                return Content($"Помилка: {ex.Message}");
            }
        }

        // --------- PDF -----------
        [HttpGet]
        public async Task<IActionResult> GradeBookPdf(int groupId, int semester)
        {
            try
            {
                var pdfBytes = await _reportService.GenerateGradeBookPdfAsync(groupId, semester);
                string fileName = $"GradeBook_{groupId}_sem{semester}.pdf";
                string contentType = "application/pdf";
                return File(pdfBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                return Content($"Помилка: {ex.Message}");
            }
        }

        // --------- WORD ----------
        [HttpGet]
        public async Task<IActionResult> GradeBookWord(int groupId, int semester)
        {
            try
            {
                var docBytes = await _reportService.GenerateGradeBookWordAsync(groupId, semester);
                string fileName = $"GradeBook_{groupId}_sem{semester}.docx";
                string contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                return File(docBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                return Content($"Помилка: {ex.Message}");
            }
        }
    }
}
