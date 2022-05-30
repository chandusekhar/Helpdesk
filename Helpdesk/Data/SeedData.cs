using Helpdesk.Infrastructure;
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
                await EnsureConfigurationOptions(context);

                // Manager
                //var managerUid = await EnsureUser(serviceProvider, "manager@demo.com", password);
                //await EnsureRole(serviceProvider, managerUid, Constants.InvoiceManagersRole);
                //// Admin
                //var adminUid = await EnsureUser(serviceProvider, "admin@demo.com", password);
                //await EnsureRole(serviceProvider, adminUid, Constants.InvoiceAdminsRole);
            }

        }

        /// <summary>
        /// Iterate over the Rollups until all patches have been applied, as identified by
        /// there being no rollup index for the current database version.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static async Task EnsureConfigurationOptions(ApplicationDbContext context)
        {
            bool Done = false;
            while (!Done)
            {
                ConfigOpt? dbVersion = await context.ConfigOpts
                    .Where(x => x.Category == ConfigOptConsts.System_Version.Category &&
                                x.Key == ConfigOptConsts.System_Version.Key)
                    .FirstOrDefaultAsync();
                if (dbVersion == null)
                {
                    dbVersion = new ConfigOpt()
                    {
                        Category = ConfigOptConsts.System_Version.Category,
                        Key = ConfigOptConsts.System_Version.Key,
                        Value = string.Empty
                    };
                }

                // find a rollup index that matches this database version
                var rollup = ConfigOptRollupCatalog.RollupIndex.Where(x => x.Version == dbVersion.Value).FirstOrDefault();
                if (rollup == null)
                {
                    Done = true;
                    break;
                }

                foreach (var item in rollup.Deletions)
                {
                    ConfigOpt? delOpt = await context.ConfigOpts
                        .Where(x => x.Category == item.Category &&
                                    x.Key == item.Key)
                    .FirstOrDefaultAsync();
                    if (delOpt != null)
                    {
                        context.ConfigOpts.Remove(delOpt);

                    }
                }
                await context.SaveChangesAsync();
                foreach (var item in rollup.Additions)
                {
                    ConfigOpt? addOpt = await context.ConfigOpts
                        .Where(x => x.Category == item.Category &&
                                    x.Key == item.Key).FirstOrDefaultAsync();

                    if (addOpt == null)
                    {
                        context.ConfigOpts.Add(new ConfigOpt()
                        {
                            Category = item.Category,
                            Key = item.Key,
                            Value = item.Value,
                            Order = item.Order
                        });
                    }
                }
                await context.SaveChangesAsync();
                foreach (var item in rollup.Changes)
                {
                    ConfigOpt? chgOpt = await context.ConfigOpts
                        .Where(x => x.Category == item.Category &&
                                    x.Key == item.Key).FirstOrDefaultAsync();
                    if (chgOpt == null)
                    {
                        context.ConfigOpts.Add(new ConfigOpt()
                        {
                            Category = item.Category,
                            Key = item.Key,
                            Value = item.Value,
                            Order = item.Order
                        });
                    }
                    else
                    {
                        chgOpt.Value = item.Value;
                        chgOpt.Order = item.Order;
                        context.ConfigOpts.Update(chgOpt);
                    }
                }
                await context.SaveChangesAsync();
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
