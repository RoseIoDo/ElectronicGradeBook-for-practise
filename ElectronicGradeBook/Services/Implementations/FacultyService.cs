using ElectronicGradeBook.Data;
using ElectronicGradeBook.Models.Entities.Core;
using ElectronicGradeBook.Models.ViewModels;
using ElectronicGradeBook.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ElectronicGradeBook.Services.Implementations
{
    public class FacultyService : IFacultyService
    {
        private readonly ApplicationDbContext _db;
        public FacultyService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<FacultyViewModel>> GetAllAsync()
        {
            return await _db.Faculties
                .OrderBy(f => f.Name)
                .Select(f => new FacultyViewModel
                {
                    Id = f.Id,
                    Name = f.Name
                })
                .ToListAsync();
        }

        public async Task<FacultyViewModel> CreateAsync(FacultyViewModel model)
        {
            // Перевірка унікальності назви
            var existName = await _db.Faculties
                .AnyAsync(f => f.Name == model.Name);
            if (existName)
                throw new Exception($"Факультет із назвою '{model.Name}' вже існує.");

            var entity = new Faculty
            {
                Name = model.Name
            };
            _db.Faculties.Add(entity);
            await _db.SaveChangesAsync();

            model.Id = entity.Id;
            return model;
        }

        public async Task<FacultyViewModel> UpdateAsync(FacultyViewModel model)
        {
            var fac = await _db.Faculties.FindAsync(model.Id);
            if (fac == null)
                throw new Exception("Факультет не знайдено.");

            // Перевірка унікальності 
            var existSameName = await _db.Faculties
                .AnyAsync(f => f.Name == model.Name && f.Id != model.Id);
            if (existSameName)
                throw new Exception($"Факультет із назвою '{model.Name}' вже існує.");

            fac.Name = model.Name;
            await _db.SaveChangesAsync();
            return model;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var fac = await _db.Faculties.FindAsync(id);
            if (fac == null)
                throw new Exception("Факультет не знайдено.");

            // Перевірка, чи є Specialty
            bool hasSpecialties = await _db.Specialties
                .AnyAsync(s => s.FacultyId == id);
            if (hasSpecialties)
                throw new Exception("Неможливо видалити факультет — він містить спеціальності.");

            _db.Faculties.Remove(fac);
            await _db.SaveChangesAsync();
            return true;
        }
    }

}
