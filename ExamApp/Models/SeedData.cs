using Domain.Tables;
using Microsoft.AspNetCore.Identity;

namespace ExamApp.Models
{
    public static class SeedData
    {
        public static async Task SeedBasicData(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Ensure roles exist
            var roles = new[] { "User", "Admin" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Check if users already exist
            if (!userManager.Users.Any())
            {
                // Normalize emails to prevent case sensitivity issues
                string userEmail = "user@gmail.com".ToLower();
                string adminEmail = "admin@gmail.com".ToLower();

                if (await userManager.FindByEmailAsync(userEmail) == null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = userEmail,
                        Email = userEmail,
                        EmailConfirmed = true
                    };

                    var userResult = await userManager.CreateAsync(user, "User@123");
                    if (userResult.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "User");
                    }
                }

                if (await userManager.FindByEmailAsync(adminEmail) == null)
                {
                    var admin = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true
                    };

                    var adminResult = await userManager.CreateAsync(admin, "Admin@123");
                    if (adminResult.Succeeded)
                    {
                        await userManager.AddToRoleAsync(admin, "Admin");
                    }
                }
            }
        }
    }


}
