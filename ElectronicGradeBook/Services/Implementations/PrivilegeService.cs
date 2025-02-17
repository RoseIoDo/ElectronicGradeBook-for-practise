using ElectronicGradeBook.Data;
using ElectronicGradeBook.Models.Entities.ActivityPrivilege;
using ElectronicGradeBook.Models.ViewModels;
using ElectronicGradeBook.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ElectronicGradeBook.Services.Implementations
{
    public class PrivilegeService : IPrivilegeService
    {
        private readonly ApplicationDbContext _db;
        public PrivilegeService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<PrivilegeViewModel>> GetAllAsync()
        {
            return await _db.Privileges
                .OrderBy(p => p.Name)
                .Select(p => new PrivilegeViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description
                })
                .ToListAsync();
        }

        public async Task<PrivilegeViewModel> CreateAsync(PrivilegeViewModel model)
        {
            var entity = new Privilege
            {
                Name = model.Name,
                Description = model.Description
            };
            _db.Privileges.Add(entity);
            await _db.SaveChangesAsync();
            model.Id = entity.Id;
            return model;
        }

        public async Task<PrivilegeViewModel> UpdateAsync(PrivilegeViewModel model)
        {
            var pr = await _db.Privileges.FindAsync(model.Id);
            if (pr == null)
                throw new Exception("Пільгу/Привілей не знайдено.");

            pr.Name = model.Name;
            pr.Description = model.Description;
            await _db.SaveChangesAsync();

            return model;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var pr = await _db.Privileges.FindAsync(id);
            if (pr == null)
                throw new Exception("Пільгу/Привілей не знайдено.");

            bool hasRefs = await _db.StudentPrivileges
                .AnyAsync(sp => sp.PrivilegeId == id);
            if (hasRefs)
                throw new Exception("Не можна видалити — є студенти з цією пільгою.");

            _db.Privileges.Remove(pr);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
