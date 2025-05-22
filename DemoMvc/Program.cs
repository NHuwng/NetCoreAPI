using Microsoft.EntityFrameworkCore;
using DemoMvc.Data;
using DemoMVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using DemoMVC.Models.Process;
using Microsoft.AspNetCore.Identity.UI.Services;
using PTPMQLMvc.Models.Process;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOptions();
var mailSettings = builder.Configuration.GetSection("MailSettings");
builder.Services.Configure<MailSettings>(mailSettings);
builder.Services.AddTransient<IEmailSender, SendMailService>();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")));
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.Configure<IdentityOptions>(Options =>
{
    //default lockout settings
    Options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    Options.Lockout.MaxFailedAccessAttempts = 5;
    Options.Lockout.AllowedForNewUsers = true;
    //config password
    Options.Password.RequireDigit = true;
    Options.Password.RequiredLength = 8;
    Options.Password.RequireNonAlphanumeric = false;
    Options.Password.RequireUppercase = true;
    Options.Password.RequireLowercase = false;
    //config login
    Options.SignIn.RequireConfirmedEmail = false;
    Options.SignIn.RequireConfirmedPhoneNumber = false;
    //config user
    Options.User.RequireUniqueEmail = true;
});
builder.Services.ConfigureApplicationCookie(Options =>
{
    Options.Cookie.HttpOnly = true;
    //chi gui cookie qua https
    Options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    //giam thieu rui ro Csrf
    Options.Cookie.SameSite = SameSiteMode.Lax;
    Options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    Options.LoginPath = "/Account/Login";
    Options.AccessDeniedPath = "/Account/AccessDenied";
    Options.SlidingExpiration = true;
});
builder.Services.AddTransient<EmployeeSeeder>();

var app = builder.Build();

// Seed dữ liệu ngay khi ứng dụng khởi động
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var seeder = services.GetRequiredService<EmployeeSeeder>();
    seeder.SeedEmployees(1000);  
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();
app.Run();
