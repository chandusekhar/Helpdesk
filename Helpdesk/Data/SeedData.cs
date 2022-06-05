using Helpdesk.Authorization;
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
                await RollupDatabase(context, serviceProvider);
            }

        }

        /// <summary>
        /// Iterate over the Rollups until all patches have been applied, as identified by
        /// there being no rollup index for the current database version.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static async Task RollupDatabase(ApplicationDbContext context, IServiceProvider serviceProvider)
        {
            bool Done = false;
            while (!Done)
            {
                // Get the current version. Will be null if there is no data at all (new database)
                ConfigOpt? dbVersion = await context.ConfigOpts
                    .Where(x => x.Category == ConfigOptConsts.System_Version.Category &&
                                x.Key == ConfigOptConsts.System_Version.Key)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
                if (dbVersion == null)
                {
                    // New database. fake a value of string.Empty, which init database updates will match.
                    dbVersion = new ConfigOpt()
                    {
                        Category = ConfigOptConsts.System_Version.Category,
                        Key = ConfigOptConsts.System_Version.Key,
                        Value = string.Empty
                    };
                }

                // Roles and claims
                foreach (var item in DefaultRoleClaimCatalog.Catalog)
                {
                    if (item.Version == dbVersion.Value)
                    {
                        List<HelpdeskClaim> roleClaims;
                        List<HelpdeskClaim> allClaims = await RightsManagement.GetAllClaims(context);

                        HelpdeskRole? role = await RightsManagement.GetRoleIfExists(context, item.RoleName);
                        if (role == null)
                        {
                            role = await RightsManagement.CreateRole(context, item.RoleName, item.RoleDescription);
                            roleClaims = new List<HelpdeskClaim>();
                        }
                        else
                        {
                            roleClaims = await RightsManagement.GetRoleClaims(context, item.RoleName);
                        }

                        foreach (var rclaim in item.Claims)
                        {
                            if (!roleClaims.Any(x => x.Name == rclaim.ClaimName))
                            {
                                var cl = allClaims.Where(x => x.Name == rclaim.ClaimName).FirstOrDefault();
                                if (cl == null)
                                {
                                    cl = await RightsManagement.CreateClaim(context, rclaim.ClaimName, rclaim.ClaimDescription);
                                    allClaims.Add(cl);
                                }
                                await RightsManagement.AddClaimToRoll(context, item.RoleName, rclaim.ClaimName);
                                roleClaims.Add(cl);
                            }
                        }
                    }
                }

                // Site navigation templates
                foreach (var item in SiteNavTemplateCatalog.Catalog)
                {
                    if (item.Version == dbVersion.Value)
                    {
                        var temp = await context.SiteNavTemplates
                            .Where(x => x.Name == item.SiteNavTemplate.Name)
                            .FirstOrDefaultAsync();
                        if (temp != null)
                        {
                            temp.Description = item.SiteNavTemplate.Description;
                            temp.TicketLink = item.SiteNavTemplate.TicketLink;
                            temp.AssetLink = item.SiteNavTemplate.AssetLink;
                            temp.PeopleLink = item.SiteNavTemplate.PeopleLink;
                            temp.ShowConfigurationMenu = item.SiteNavTemplate.ShowConfigurationMenu;
                            temp.LicenseTypeLink = item.SiteNavTemplate.LicenseTypeLink;
                            temp.SiteSettingsLink = item.SiteNavTemplate.SiteSettingsLink;

                            context.SiteNavTemplates.Update(temp);
                            await context.SaveChangesAsync();
                        }
                        else
                        {
                            context.SiteNavTemplates.Add(item.SiteNavTemplate);
                            await context.SaveChangesAsync();
                        }
                    }
                }

                // License Types
                foreach (var item in LicenseTypeCatalog.Catalog)
                {
                    if (item.Version == dbVersion.Value)
                    {
                        foreach (var lic in item.Templates)
                        {
                            var temp = await context.LicenseType
                                .Where(x => x.Name == lic.Name)
                                .FirstOrDefaultAsync();
                            if (temp != null)
                            {
                                temp.Description = lic.Description;
                                temp.Seats = lic.Seats;
                                temp.IsDeviceLicense = lic.IsDeviceLicense;
                                temp.IsUserLicense = lic.IsUserLicense;
                                temp.DeviceRequireProductCode = lic.DeviceRequireProductCode;
                                temp.UserRequireProductCode = lic.UserRequireProductCode;

                                context.LicenseType.Update(temp);
                            }
                            else
                            {
                                temp = new LicenseType()
                                {
                                    Name = lic.Name,
                                    Description = lic.Description,
                                    Seats = lic.Seats,
                                    IsDeviceLicense = lic.IsDeviceLicense,
                                    IsUserLicense = lic.IsUserLicense,
                                    DeviceRequireProductCode = lic.DeviceRequireProductCode,
                                    UserRequireProductCode = lic.UserRequireProductCode
                                };
                                context.LicenseType.Add(temp);
                            }
                            await context.SaveChangesAsync();
                        }
                    }
                }

                // site settings.
                var rollup = ConfigOptRollupCatalog.RollupIndex.Where(x => x.Version == dbVersion.Value).FirstOrDefault();
                if (rollup == null)
                {
                    Done = true;
                    break;
                }
                // Remove old entries first.
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
                // Add new entries
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
                            Order = item.Order,
                            ReferenceType = item.ReferenceType,
                        });
                    }
                }
                await context.SaveChangesAsync();
                // update existing entries
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
                            Order = item.Order,
                            ReferenceType = item.ReferenceType
                        });
                    }
                    else
                    {
                        chgOpt.Value = item.Value;
                        chgOpt.Order = item.Order;
                        chgOpt.ReferenceType = item.ReferenceType;
                        context.ConfigOpts.Update(chgOpt);
                    }
                }
                await context.SaveChangesAsync();
            }
        }


        //private static async Task<string> EnsureUser(
        //    IServiceProvider serviceProvider,
        //    string userName,
        //    string initPwd)
        //{
        //    var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();
        //    if (userManager == null)
        //    {
        //        throw new Exception("UserManager is not registered.");
        //    }

        //    var user = await userManager.FindByNameAsync(userName);
        //    if (user == null)
        //    {
        //        user = new IdentityUser
        //        {
        //            UserName = userName,
        //            Email = userName,
        //            EmailConfirmed = true
        //        };
        //        var result = await userManager.CreateAsync(user, initPwd);
        //        // maybe need to catch an error here if password is weak.
        //    }

        //    if (user == null)
        //    {
        //        throw new Exception("User did not get created. Check the password policy.");
        //    }

        //    return user.Id;
        //}

        //private static async Task<IdentityResult> EnsureRole(
        //    IServiceProvider serviceProvider,
        //    string userId, string role)
        //{
        //    var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();
        //    if (roleManager == null)
        //    {
        //        throw new Exception("Role Manager is not registered.");
        //    }

        //    IdentityResult iresult;

        //    if (await roleManager.RoleExistsAsync(role) == false)
        //    {
        //        iresult = await roleManager.CreateAsync(new IdentityRole(role));
        //    }

        //    var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();
        //    if (userManager == null)
        //    {
        //        throw new Exception("UserManager is not registered.");
        //    }

        //    var user = await userManager.FindByIdAsync(userId);
        //    if (user == null)
        //    {
        //        throw new Exception("User does not exist.");
        //    }
        //    iresult = await userManager.AddToRoleAsync(user, role);
        //    return iresult;
        //}
    }
    public class ConfigOptDefault
    {
        public ConfigOptDefault(string category, string key)
        {
            Category = category;
            Key = key;
            Value = string.Empty;
            Order = null;
            ReferenceType = ReferenceTypes.Hidden;
        }

        public ConfigOptDefault(string category, string key, string value, int order, ReferenceTypes type)
        {
            Category = category;
            Key = key;
            Value = value;
            Order = order;
            ReferenceType = type;
        }

        public string Category { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public int? Order { get; set; }
        public ReferenceTypes ReferenceType { get; set; }
    }

    /// <summary>
    /// Provides a list of options that need to be applied if the database is currently on a specified version.
    /// </summary>
    public static class ConfigOptRollupCatalog
    {
        public static List<ConfigOptVersion> RollupIndex = new List<ConfigOptVersion>()
        {
            new ConfigOptVersion()
            {
                Version = string.Empty,
                Additions = new List<ConfigOptDefault>()
                {
                    ConfigOptConsts.System_Version,
                    ConfigOptConsts.Accounts_AllowSelfRegistration,
                    ConfigOptConsts.Accounts_MfaQrCodeSitename,
                    ConfigOptConsts.Accounts_ShowMfaBanner,
                    ConfigOptConsts.Accounts_DefaultNavTemplate,
                    ConfigOptConsts.Branding_OrganizationName,
                    ConfigOptConsts.Branding_SiteName,
                    ConfigOptConsts.Branding_SiteURL
                }
            }
        };
    }

    public class ConfigOptVersion
    {
        public ConfigOptVersion()
        {
            Version = string.Empty;
            Additions = new List<ConfigOptDefault>();
            Deletions = new List<ConfigOptDefault>();
            Changes = new List<ConfigOptDefault>();
        }

        /// <summary>
        /// Version of the database that this update applies to.
        /// </summary>
        public string Version { get; set; }
        public List<ConfigOptDefault> Additions;
        public List<ConfigOptDefault> Deletions;
        public List<ConfigOptDefault> Changes;
    }

    public class DefaultRoleClaim
    {
        /// <summary>
        /// The version of the database that will trigger this role be created.
        /// string.Empty is for a new database.
        /// </summary>
        public string Version { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public List<NewRoleClaim> Claims { get; set; }

        public class NewRoleClaim
        {
            public NewRoleClaim(string claimName, string claimDescription)
            {
                ClaimName = claimName;
                ClaimDescription = claimDescription;
            }
            public string ClaimName { get; set; }
            public string ClaimDescription { get; set; }
        }

    }

    public static class DefaultRoleClaimCatalog
    {
        public static List<DefaultRoleClaim> Catalog = new List<DefaultRoleClaim>()
        {
            new DefaultRoleClaim()
            {
                Version = string.Empty,
                RoleName = RoleConstantStrings.SuperAdmin,
                RoleDescription = "Super Admins can do anything on the site",
                Claims = new List<DefaultRoleClaim.NewRoleClaim>()
                {
                    new DefaultRoleClaim.NewRoleClaim(ClaimConstantStrings.RolesCreateNew,
                        "Allows creating a new Role. Requires RolesViewClaims, RolesEditClaims"),
                    new DefaultRoleClaim.NewRoleClaim(ClaimConstantStrings.RolesViewClaims,
                        "Allows viewing a Role's claims"),
                    new DefaultRoleClaim.NewRoleClaim(ClaimConstantStrings.RolesEditClaims,
                        "Allows Editing a Role's claims"),
                    new DefaultRoleClaim.NewRoleClaim(ClaimConstantStrings.RolesDeleteRole,
                        "Allows deleting a role"),
                    new DefaultRoleClaim.NewRoleClaim(ClaimConstantStrings.LicenseTypeAdmin,
                        "Allows creating, editing, and removing License Types"),
                    new DefaultRoleClaim.NewRoleClaim(ClaimConstantStrings.SitewideConfigurationEditor,
                        "Allows editing sitewide configuration settings"),
                    new DefaultRoleClaim.NewRoleClaim(ClaimConstantStrings.UsersAdmin,
                        "Allows creating users, resetting passwords for users, enabling/disabling users"),
                    new DefaultRoleClaim.NewRoleClaim(ClaimConstantStrings.UsersRolesAdmin,
                        "Allows granting/revoking roles for users. Requires UsersAdmin to get to the page to do this"),
                    //new DefaultRoleClaim.NewRoleClaim(ClaimConstantStrings.UsersPrivilegedAdmin,
                    //    "Allows password reset, enabling/disabling of users with privileged roles"),
                    //new DefaultRoleClaim.NewRoleClaim(ClaimConstantStrings.UsersPrivilegedRolesAdmin,
                    //    "Allows adding a privileged role to a user (super admin, for example). Requires UsersAdmin to get to the page to do this"),
                    new DefaultRoleClaim.NewRoleClaim(ClaimConstantStrings.UsersAllowReadAccess,
                        "Required to access the users screen and view basic user details"),
                }
            },
            new DefaultRoleClaim()
            {
                Version = string.Empty,
                RoleName = RoleConstantStrings.UserAdmin,
                RoleDescription = "User Admins can create users, reset user passwords and security options, and enable/disable accounts",
                Claims = new List<DefaultRoleClaim.NewRoleClaim>()
                {
                    new DefaultRoleClaim.NewRoleClaim(ClaimConstantStrings.UsersAdmin,
                        "Allows creating users, resetting passwords for users, enabling/disabling users"),
                    new DefaultRoleClaim.NewRoleClaim(ClaimConstantStrings.UsersRolesAdmin,
                        "Allows granting/revoking roles for users. Requires UsersAdmin to get to the page to do this"),
                    new DefaultRoleClaim.NewRoleClaim(ClaimConstantStrings.UsersAllowReadAccess,
                        "Required to access the users screen and view basic user details"),
                }
            }
        };
    }
    
    public class SiteNavTemplateVersion
    {
        public string Version { get; set; }
        public SiteNavTemplate SiteNavTemplate { get; set; }
    }

    public static class SiteNavTemplateCatalog
    {
        public static List<SiteNavTemplateVersion> Catalog = new List<SiteNavTemplateVersion>()
        {
            new SiteNavTemplateVersion()
            {
                Version = "",
                SiteNavTemplate = new SiteNavTemplate()
                {
                    Name = "Everything Visible",
                    Description = "All Navbar links are shown regardless of access level for user",
                    TicketLink = true,
                    AssetLink = true,
                    PeopleLink = true,
                    ShowConfigurationMenu = true,
                    SiteSettingsLink = true,
                    LicenseTypeLink = true
                }
            }
        };
    }

    public class LicenseTypeTemplateVersion
    {
        public string Version { get; set; }
        public List<LicenseType> Templates { get; set; }
    }

    public static class LicenseTypeCatalog
    {
        public static List<LicenseTypeTemplateVersion> Catalog = new List<LicenseTypeTemplateVersion>()
        {
            new LicenseTypeTemplateVersion()
            {
                Version = "",
                Templates = new List<LicenseType>()
                {
                    new LicenseType()
                    {
                    Name = "Exchange Online (Plan 1) for GCC",
                    Description = "Exchange Email license for Government Community Cloud",
                    Seats = null,
                    DeviceRequireProductCode = false,
                    UserRequireProductCode = false,
                    IsDeviceLicense = false,
                    IsUserLicense = true
                    },
                    new LicenseType()
                    {
                        Name = "Office 365 G3 GCC",
                        Description = "Office Desktop Products, Email, SharePoint, and Teams for Government Community Cloud",
                        Seats = null,
                        DeviceRequireProductCode = false,
                        UserRequireProductCode = false,
                        IsDeviceLicense = false,
                        IsUserLicense = true
                    },
                    new LicenseType()
                    {
                        Name = "Microsoft 365 Audio Conferencing for GCC",
                        Description = "Teams Audio Conferencing license add-on for Office 365 G3 GCC",
                        Seats = null,
                        DeviceRequireProductCode = false,
                        UserRequireProductCode = false,
                        IsDeviceLicense = false,
                        IsUserLicense = true
                    },
                    new LicenseType()
                    {
                        Name = "Project Plan 3 for GCC",
                        Description = "Microsoft Project Online desktop software for Government Community Cloud",
                        Seats = null,
                        DeviceRequireProductCode = false,
                        UserRequireProductCode = false,
                        IsDeviceLicense = false,
                        IsUserLicense = true
                    },
                    new LicenseType()
                    {
                        Name = "Visio Plan 2 for GCC",
                        Description = "Microsoft Visio desktop software for Government Community Cloud",
                        Seats = null,
                        DeviceRequireProductCode = false,
                        UserRequireProductCode = false,
                        IsDeviceLicense = false,
                        IsUserLicense = true
                    },
                    new LicenseType()
                    {
                        Name = "Adobe Creative Cloud",
                        Description = "Adobe Photoshop, Acrobat Pro, Premiere Pro, Illustrator, Lightroom, and InDesign",
                        Seats = null,
                        DeviceRequireProductCode = false,
                        UserRequireProductCode = false,
                        IsDeviceLicense = false,
                        IsUserLicense = true
                    },
                    new LicenseType()
                    {
                        Name = "Adobe Creative Cloud",
                        Description = "Adobe Acrobat Pro DC desktop software",
                        Seats = null,
                        DeviceRequireProductCode = false,
                        UserRequireProductCode = false,
                        IsDeviceLicense = false,
                        IsUserLicense = true
                    },
                    new LicenseType()
                    {
                        Name = "Smartsheet",
                        Description = "Project management software",
                        Seats = null,
                        DeviceRequireProductCode = false,
                        UserRequireProductCode = false,
                        IsDeviceLicense = false,
                        IsUserLicense = true
                    },
                    new LicenseType()
                    {
                        Name = "Trend Micro Worry-Free Business",
                        Description = "Antivirus and endpoint protection software",
                        Seats = null,
                        DeviceRequireProductCode = false,
                        UserRequireProductCode = false,
                        IsDeviceLicense = true,
                        IsUserLicense = false
                    },
                    new LicenseType()
                    {
                        Name = "Avast Business Pro",
                        Description = "Antivirus and endpoint protection software",
                        Seats = null,
                        DeviceRequireProductCode = false,
                        UserRequireProductCode = false,
                        IsDeviceLicense = true,
                        IsUserLicense = false
                    },
                    new LicenseType()
                    {
                        Name = "Windows 10 Professional Upgrade",
                        Description = "Upgrade license for Windows 10 Home to Professional",
                        Seats = null,
                        DeviceRequireProductCode = true,
                        UserRequireProductCode = false,
                        IsDeviceLicense = true,
                        IsUserLicense = false
                    },
                    new LicenseType()
                    {
                        Name = "Windows 11 Professional Upgrade",
                        Description = "Upgrade license for Windows 11 Home to Professional",
                        Seats = null,
                        DeviceRequireProductCode = true,
                        UserRequireProductCode = false,
                        IsDeviceLicense = true,
                        IsUserLicense = false
                    },
                    new LicenseType()
                    {
                        Name = "Windows Server 2019 Standard Volume License",
                        Description = "Server operating system",
                        Seats = null,
                        DeviceRequireProductCode = true,
                        UserRequireProductCode = false,
                        IsDeviceLicense = true,
                        IsUserLicense = false
                    },
                    new LicenseType()
                    {
                        Name = "Windows Server 2019 Datacenter Volume License",
                        Description = "Server operating system",
                        Seats = null,
                        DeviceRequireProductCode = true,
                        UserRequireProductCode = false,
                        IsDeviceLicense = true,
                        IsUserLicense = false
                    }
                }
            }
        };
    }
}
