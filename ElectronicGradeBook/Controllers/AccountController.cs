using ElectronicGradeBook.Models.Entities.Security;
using ElectronicGradeBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ElectronicGradeBook.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Створюємо користувача
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName, // Прийдемо з RegisterViewModel
                IsActive = true
            };

            // Створити в AspNetUsers із паролем
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // Наприклад, одразу призначити роль "Admin" або "Student" (за потреби)
                // await EnsureRoleExists("Admin"); // створення ролі якщо ще немає
                // await _userManager.AddToRoleAsync(user, "Admin");

                // Логінимо щойно створеного користувача
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Виводимо помилки (наприклад, слабкий пароль)
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                return View(model);
            }
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Знаходимо користувача
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Неправильний Email або пароль.");
                return View(model);
            }

            // Перевіряємо, чи він активний
            if (!user.IsActive)
            {
                ModelState.AddModelError("", "Ваш обліковий запис неактивний.");
                return View(model);
            }

            // Спробуємо пароль
            var result = await _signInManager.PasswordSignInAsync(
                user.UserName,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // Якщо є returnUrl і воно локальне, переходимо туди, інакше на Home
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Невдала спроба логіну.");
                return View(model);
            }
        }

        // GET: /Account/Logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // Приклад - призначити роль уже існуючому користувачеві
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AssignRole(string userEmail, string roleName)
        {
            // тільки Admin може призначати ролі
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
                return NotFound("Користувач не знайдений");

            // Створити роль, якщо її немає
            await EnsureRoleExists(roleName);

            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (result.Succeeded)
            {
                return Ok($"Role '{roleName}' призначено користувачеві {userEmail}");
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        // Допоміжний метод для створення ролі, якщо її немає
        private async Task EnsureRoleExists(string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
}
