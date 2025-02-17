using System.Threading.Tasks;

namespace ElectronicGradeBook.Services.Interfaces
{
    public interface IReportService
    {
        // 1) Excel
        Task<byte[]> GenerateGradeBookExcelAsync(int groupId, int semester);

        // 2) PDF
        Task<byte[]> GenerateGradeBookPdfAsync(int groupId, int semester);

        // 3) Word
        Task<byte[]> GenerateGradeBookWordAsync(int groupId, int semester);
    }
}