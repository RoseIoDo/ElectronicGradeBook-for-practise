using Microsoft.AspNetCore.Authentication.Cookies;
using ElectronicGradeBook.Data;
using Microsoft.EntityFrameworkCore;
using ElectronicGradeBook.Services.Interfaces;
using ElectronicGradeBook.Services.Implementations;
using Microsoft.AspNetCore.Identity;             // <-- Додайте
using ElectronicGradeBook.Models.Entities.Security; // <-- Додайте, щоб бачити ApplicationUser

var builder = WebApplication.CreateBuilder(args);

// 1) DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2) Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Якщо isAuthEnabled=true, ви додаєте cookie-автентифікацію
bool isAuthEnabled = builder.Configuration.GetValue<bool>("AuthDemoSettings:IsEnabled", true);
if (isAuthEnabled)
{
    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.LoginPath = "/Auth/Login";
            options.AccessDeniedPath = "/Auth/AccessDenied";
        });
    builder.Services.AddAuthorization();
}

// MVC
builder.Services.AddControllersWithViews();

// РЕЄСТРУЄМО ВСІ СЕРВІСИ 
builder.Services.AddScoped<IFacultyService, FacultyService>();
builder.Services.AddScoped<IStudyProgramService, StudyProgramService>();
builder.Services.AddScoped<ISpecialtyService, SpecialtyService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<ISubjectOfferingService, SubjectOfferingService>();
builder.Services.AddScoped<IGradeService, GradeService>();
builder.Services.AddScoped<IActivityService, ActivityService>();
builder.Services.AddScoped<IStudentActivityService, StudentActivityService>();
builder.Services.AddScoped<IPrivilegeService, PrivilegeService>();
builder.Services.AddScoped<IStudentPrivilegeService, StudentPrivilegeService>();
builder.Services.AddScoped<ISubjectSubgroupService, SubjectSubgroupService>();


builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IGradeBookService, GradeBookService>();

var app = builder.Build();

// (необов'язково) створимо роль "Admin" якщо її немає
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
