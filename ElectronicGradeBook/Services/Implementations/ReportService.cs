using ClosedXML.Excel;
using ElectronicGradeBook.Data;
using ElectronicGradeBook.Models.Entities.Core;
using ElectronicGradeBook.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;         
using QuestPDF.Infrastructure;  
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xceed.Words.NET;           
using System;

namespace ElectronicGradeBook.Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _db;

        public ReportService(ApplicationDbContext db)
        {
            _db = db;
        }

        // ----------------- СПІЛЬНА ЛОГІКА ЗБОРУ ДАНИХ -----------------

        private class GradeBookRecord
        {
            public string StudentName { get; set; }
            public string SubjectName { get; set; }
            public string Status { get; set; }
            public bool IsRetake { get; set; }
            public DateTime DateUpdated { get; set; }
        }

        /// <summary>
        /// Повертає назву групи і колекцію записів (студент, предмет, оцінка)
        /// для зазначеного groupId і semester
        /// </summary>
        private async Task<(string groupDisplay, List<GradeBookRecord>)> GetGradeBookData(int groupId, int semester)
        {
            // Шукаємо групу
            var group = await _db.Groups
                .FirstOrDefaultAsync(g => g.Id == groupId);
            if (group == null)
                throw new Exception("Групу не знайдено.");

            string groupName = $"{group.GroupPrefix}-{group.GroupNumber}";

            // Тягнемо оцінки
            var gradesQuery = _db.Grades
                .Include(g => g.Student)
                .Include(g => g.SubjectOffering).ThenInclude(o => o.Subject)
                .Where(g => g.Student.GroupId == groupId
                         && g.SubjectOffering.SemesterInYear == semester);

            var gradeList = await gradesQuery.ToListAsync();

            var result = gradeList
                .Select(gr => new GradeBookRecord
                {
                    StudentName = gr.Student.FullName,
                    SubjectName = gr.SubjectOffering.Subject.FullName,
                    Status = gr.Status,
                    IsRetake = gr.IsRetake,
                    DateUpdated = gr.DateUpdated
                })
                .OrderBy(r => r.StudentName)
                .ThenBy(r => r.SubjectName)
                .ToList();

            return (groupName, result);
        }

        // ----------------- 1) EXCEL -----------------
        public async Task<byte[]> GenerateGradeBookExcelAsync(int groupId, int semester)
        {
            var (groupDisplay, data) = await GetGradeBookData(groupId, semester);

            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("GradeBook");

                ws.Cell(1, 1).Value = $"Залікова відомість групи: {groupDisplay}, Семестр: {semester}";
                ws.Range(1, 1, 1, 5).Merge().Style.Font.Bold = true;

                // Заголовки
                ws.Cell(2, 1).Value = "Студент";
                ws.Cell(2, 2).Value = "Предмет";
                ws.Cell(2, 3).Value = "Оцінка (Status)";
                ws.Cell(2, 4).Value = "Перездача?";
                ws.Cell(2, 5).Value = "Дата оновл.";

                int row = 3;
                foreach (var rec in data)
                {
                    ws.Cell(row, 1).Value = rec.StudentName;
                    ws.Cell(row, 2).Value = rec.SubjectName;
                    ws.Cell(row, 3).Value = rec.Status;
                    ws.Cell(row, 4).Value = rec.IsRetake ? "Так" : "Ні";
                    ws.Cell(row, 5).Value = rec.DateUpdated.ToString("yyyy-MM-dd");
                    row++;
                }

                ws.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        // ----------------- 2) PDF (QuestPDF) -----------------
        public async Task<byte[]> GenerateGradeBookPdfAsync(int groupId, int semester)
        {
            var (groupDisplay, data) = await GetGradeBookData(groupId, semester);

            // Для PDF із QuestPDF будуємо документ:
            var doc = Document.Create(container =>
            {
                container.Page(page => {
                    page.Margin(30);
                    page.Header()
                        .Text($"Залікова відомість групи: {groupDisplay}, Семестр: {semester}")
                        .Bold().FontSize(16);

                    page.Content().Table(table =>
                    {
                        // Шапка
                        table.ColumnsDefinition(cols =>
                        {
                            cols.ConstantColumn(150); // Student
                            cols.ConstantColumn(150); // Subject
                            cols.ConstantColumn(100); // Status
                            cols.ConstantColumn(70);  // IsRetake
                            cols.ConstantColumn(100); // DateUpdated
                        });

                        // Заголовок рядка
                        table.Header(header =>
                        {
                            header.Cell().Text("Студент").Bold();
                            header.Cell().Text("Предмет").Bold();
                            header.Cell().Text("Оцінка").Bold();
                            header.Cell().Text("Перездача?").Bold();
                            header.Cell().Text("Дата оновл.").Bold();
                        });

                        // Рядки
                        foreach (var rec in data)
                        {
                            table.Cell().Text(rec.StudentName);
                            table.Cell().Text(rec.SubjectName);
                            table.Cell().Text(rec.Status);
                            table.Cell().Text(rec.IsRetake ? "Так" : "Ні");
                            table.Cell().Text(rec.DateUpdated.ToString("yyyy-MM-dd"));
                        }
                    });
                });
            });

            // Рендеримо в byte[]
            var pdfBytes = doc.GeneratePdf();
            return pdfBytes;
        }

        // ----------------- 3) WORD (Xceed.Words.NET) -----------------
        public async Task<byte[]> GenerateGradeBookWordAsync(int groupId, int semester)
        {
            var (groupDisplay, data) = await GetGradeBookData(groupId, semester);

            using (var memStream = new MemoryStream())
            {
                // створюємо документ у пам'яті
                using (var doc = DocX.Create(memStream))
                {
                    doc.InsertParagraph($"Залікова відомість групи: {groupDisplay}, Семестр: {semester}")
                        .Bold().FontSize(16d)
                        .SpacingAfter(15d);

                    // Створимо таблицю: data.Count + 1 (рядок заголовків) x 5 (стовпців)
                    int rowCount = data.Count + 1;
                    int colCount = 5;
                    var table = doc.AddTable(rowCount, colCount);

                    // Заголовки:
                    table.Rows[0].Cells[0].Paragraphs[0].Append("Студент").Bold();
                    table.Rows[0].Cells[1].Paragraphs[0].Append("Предмет").Bold();
                    table.Rows[0].Cells[2].Paragraphs[0].Append("Оцінка").Bold();
                    table.Rows[0].Cells[3].Paragraphs[0].Append("Перездача?").Bold();
                    table.Rows[0].Cells[4].Paragraphs[0].Append("Дата оновл.").Bold();

                    // Заповнення
                    int r = 1;
                    foreach (var rec in data)
                    {
                        table.Rows[r].Cells[0].Paragraphs[0].Append(rec.StudentName);
                        table.Rows[r].Cells[1].Paragraphs[0].Append(rec.SubjectName);
                        table.Rows[r].Cells[2].Paragraphs[0].Append(rec.Status);
                        table.Rows[r].Cells[3].Paragraphs[0].Append(rec.IsRetake ? "Так" : "Ні");
                        table.Rows[r].Cells[4].Paragraphs[0].Append(rec.DateUpdated.ToString("yyyy-MM-dd"));
                        r++;
                    }

                    doc.InsertTable(table);
                    doc.Save();
                }

                return memStream.ToArray();
            }
        }
    }
}
