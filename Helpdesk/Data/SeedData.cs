using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk.Data
{
    public class SeedData
    {
        public static async Task Initialize(
            IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // Manager
                //var managerUid = await EnsureUser(serviceProvider, "manager@demo.com", password);
                //await EnsureRole(serviceProvider, managerUid, Constants.InvoiceManagersRole);
                //// Admin
                //var adminUid = await EnsureUser(serviceProvider, "admin@demo.com", password);
                //await EnsureRole(serviceProvider, adminUid, Constants.InvoiceAdminsRole);
            }

        }

        private static async Task<string> EnsureUser(
            IServiceProvider serviceProvider,
            string userName,
            string initPwd)
        {
            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();
            if (userManager == null)
            {
                throw new Exception("UserManager is not registered.");
            }

            var user = await userManager.FindByNameAsync(userName);
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = userName,
                    Email = userName,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(user, initPwd);
                // maybe need to catch an error here if password is weak.
            }

            if (user == null)
            {
                throw new Exception("User did not get created. Check the password policy.");
            }

            return user.Id;
        }

        private static async Task<IdentityResult> EnsureRole(
            IServiceProvider serviceProvider,
            string userId, string role)
        {
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();
            if (roleManager == null)
            {
                throw new Exception("Role Manager is not registered.");
            }

            IdentityResult iresult;

            if (await roleManager.RoleExistsAsync(role) == false)
            {
                iresult = await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();
            if (userManager == null)
            {
                throw new Exception("UserManager is not registered.");
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User does not exist.");
            }
            iresult = await userManager.AddToRoleAsync(user, role);
            return iresult;
        }
    }
}
