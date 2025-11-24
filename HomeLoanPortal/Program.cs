using HomeLoanPortal.Data;
using HomeLoanPortal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------------------------------
// 1. DATABASE
// ----------------------------------------------------------------------
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ----------------------------------------------------------------------
// 2. IDENTITY + COOKIES
// ----------------------------------------------------------------------
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password Policy
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;

    // User settings
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Authentication Cookie Config
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.Name = "HomeLoanPortal.Auth";

    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";

    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(5);
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// ----------------------------------------------------------------------
// 3. MIDDLEWARE PIPELINE
// ----------------------------------------------------------------------
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// ----------------------------------------------------------------------
// 4. ADMIN ROLE + ADMIN USER SEEDING
// ----------------------------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // Create Admin Role if not exists
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    // Admin seed details
    string adminEmail = "admin@homeloan.com";
    string adminPassword = "Admin@123";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        // NOTE: Your DB table requires these fields to not be null
        var admin = new ApplicationUser
        {
            FirstName = "System",
            LastName = "Admin",
            Gender = "Other",
            Nationality = "Indian",
            PhoneNumber = "9999999999",

            UserName = adminEmail,
            Email = adminEmail
        };

        var result = await userManager.CreateAsync(admin, adminPassword);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}

// ----------------------------------------------------------------------
// 5. ROUTING
// ----------------------------------------------------------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
