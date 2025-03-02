using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Domain;
using Domain.Tables;
using Bl.services;
using Microsoft.EntityFrameworkCore.Metadata;
using Bl.Infrastructure;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ExamAppDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddScoped<IClsUser, ClsUser>();
builder.Services.AddScoped<IClsExam, ClsExam>();






builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ApplicationUser,IdentityRole>()
    .AddEntityFrameworkStores<ExamAppDbContext>().AddDefaultTokenProviders(); ;



builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    await SeedData(userManager, roleManager);
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}");

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    app.MapRazorPages();
});

app.Run();

async Task SeedData(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
{
    // Create roles if they do not exist
    if (!await roleManager.RoleExistsAsync("User"))
        await roleManager.CreateAsync(new IdentityRole("User"));

    if (!await roleManager.RoleExistsAsync("Admin"))
        await roleManager.CreateAsync(new IdentityRole("Admin"));

    // Check if any users exist in the database
    if (!userManager.Users.Any())
    {
        // Create User with Role "User"
        var user = new ApplicationUser
        {
            UserName = "user@Gmail.com",
            Email = "user@Gmail.com",
            EmailConfirmed = true
        };

        var userResult = await userManager.CreateAsync(user, "User@123");
        if (userResult.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "User");
        }

        // Create Admin with Role "Admin"
        var admin = new ApplicationUser
        {
            UserName = "admin@Gmail.com",
            Email = "admin@Gmail.com",
            EmailConfirmed = true
        };

        var adminResult = await userManager.CreateAsync(admin, "Admin@123");
        if (adminResult.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}