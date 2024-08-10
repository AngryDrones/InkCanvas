using InkCanvas.Models;
using Microsoft.AspNetCore.Identity;

namespace InkCanvas
{
    public class RoleInitializer
    {
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole>
        roleManager)
        {
            string adminEmail = "admin@gmail.com";
            string password = "Admin1_";
            string adminLogin = "Admin";
            if (await roleManager.FindByNameAsync("SuperAdmin") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
            }
            if (await roleManager.FindByNameAsync("Admin") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (await roleManager.FindByNameAsync("User") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }
            if (await userManager.FindByNameAsync(adminEmail) == null)
            {
                User admin = new User { Email = adminEmail, UserName = adminEmail, Login = adminLogin };
                IdentityResult result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "User");
                    await userManager.AddToRoleAsync(admin, "Admin");
                    await userManager.AddToRoleAsync(admin, "SuperAdmin");
                }
            }
        }
    }
}