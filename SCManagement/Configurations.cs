using SCManagement.Models;
using Microsoft.AspNetCore.Identity;

namespace SCManagement {
    public class Configurations {
        public static async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            string[] roleNames = { "Administrator", "Regular" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist) await roleManager.CreateAsync(new IdentityRole(roleName));
            }

            //create a default user just to use the app
            var regularUser = new User { FirstName = "Regular", LastName = "User", UserName = "user@scmanagement.me", Email = "user@scmanagement.me", EmailConfirmed = true };
            var findRegularUser = await userManager.FindByEmailAsync(regularUser.Email);

            //if the user doesn't existe, create it
            if (findRegularUser == null)
            {
                var createPowerUser = await userManager.CreateAsync(regularUser, "User123!");
                if (createPowerUser.Succeeded) await userManager.AddToRoleAsync(regularUser, "Regular");
            }

            // create user named admin with admin role
            var adminUser = new User { FirstName = "Admin", LastName = "User", UserName = "admin@scmanagement.me", Email = "admin@scmanagement.me", EmailConfirmed = true };
            var findAdminUser = await userManager.FindByEmailAsync(adminUser.Email);

            //if the user doesn't existe, create it
            if (findAdminUser == null)
            {
                var creteAdminUser = await userManager.CreateAsync(adminUser, "Admin123!");
                if (creteAdminUser.Succeeded) await userManager.AddToRoleAsync(adminUser, "Administrator");
            }
        }
    }
}
