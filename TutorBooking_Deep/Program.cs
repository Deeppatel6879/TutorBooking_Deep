using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TutorBooking.DataAccess.Data;
using TutorBooking.DataAccess.Repository;
using TutorBooking.DataAccess.Repository.IRepository;

var builder = WebApplication.CreateBuilder(args);

// -------------------------------------------------
// DATABASE CONNECTION
// -------------------------------------------------

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Connection string 'DefaultConnection' was not found.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));


// -------------------------------------------------
// IDENTITY
// -------------------------------------------------

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;

    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();


// -------------------------------------------------
// LOGIN / ACCESS DENIED PATHS
// -------------------------------------------------

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});


// -------------------------------------------------
// REPOSITORY / UNIT OF WORK
// -------------------------------------------------

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


// -------------------------------------------------
// MVC
// -------------------------------------------------

builder.Services.AddControllersWithViews();

var app = builder.Build();


// -------------------------------------------------
// HTTP PIPELINE
// -------------------------------------------------

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


// -------------------------------------------------
// CREATE ROLES AND DEVELOPMENT ACCOUNTS
// -------------------------------------------------

using (var scope = app.Services.CreateScope())
{
    var roleManager =
        scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole>>();

    var userManager =
        scope.ServiceProvider
            .GetRequiredService<UserManager<IdentityUser>>();

    string[] roles =
    {
        "Student",
        "Tutor",
        "Admin"
    };

    // Create roles
    foreach (string role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(
                new IdentityRole(role));
        }
    }


    // -------------------------------------------------
    // ADMIN ACCOUNT
    // -------------------------------------------------

    string adminEmail =
        "admin@tutorbooking.local";

    string adminPassword =
        "Admin123";

    IdentityUser? adminUser =
        await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var adminResult =
            await userManager.CreateAsync(
                adminUser,
                adminPassword);

        if (adminResult.Succeeded)
        {
            await userManager.AddToRoleAsync(
                adminUser,
                "Admin");
        }
    }
    else
    {
        if (!await userManager.IsInRoleAsync(
            adminUser,
            "Admin"))
        {
            await userManager.AddToRoleAsync(
                adminUser,
                "Admin");
        }
    }


    // -------------------------------------------------
    // TUTOR ACCOUNT
    // -------------------------------------------------

    string tutorEmail =
        "tutor@tutorbooking.local";

    string tutorPassword =
        "Tutor123";

    IdentityUser? tutorUser =
        await userManager.FindByEmailAsync(tutorEmail);

    if (tutorUser == null)
    {
        tutorUser = new IdentityUser
        {
            UserName = tutorEmail,
            Email = tutorEmail,
            EmailConfirmed = true
        };

        var tutorResult =
            await userManager.CreateAsync(
                tutorUser,
                tutorPassword);

        if (tutorResult.Succeeded)
        {
            await userManager.AddToRoleAsync(
                tutorUser,
                "Tutor");
        }
    }
    else
    {
        if (!await userManager.IsInRoleAsync(
            tutorUser,
            "Tutor"))
        {
            await userManager.AddToRoleAsync(
                tutorUser,
                "Tutor");
        }
    }
}


// -------------------------------------------------
// ROUTING
// -------------------------------------------------

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();