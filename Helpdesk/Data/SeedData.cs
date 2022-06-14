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
                            temp.GroupsLink = item.SiteNavTemplate.GroupsLink;
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
                                    UserRequireProductCode = lic.UserRequireProductCode,
                                    Status = lic.Status
                                };
                                context.LicenseType.Add(temp);
                            }
                            await context.SaveChangesAsync();
                            context.Entry(temp).State = EntityState.Detached;
                        }
                    }
                }

                // groups
                foreach (var item in GroupCatalog.Catalog)
                {
                    if (item.Version == dbVersion.Value)
                    {
                        foreach (var g in item.Groups)
                        {
                            var group = await context.Groups.Where(x => x.Name == g.Name).FirstOrDefaultAsync();
                            if (group != null && group.Description != g.Description)
                            {
                                group.Description = g.Description;
                                context.Groups.Update(group);
                            }
                            else
                            {
                                group = new Group()
                                {
                                    Name = g.Name,
                                    Description = g.Description
                                };
                                context.Groups.Add(group);
                            }
                            await context.SaveChangesAsync();
                            context.Entry(group).State = EntityState.Detached;
                        }
                    }
                }

                // Document Types
                foreach (var item in DocumentTypeCatalog.Catalog)
                {
                    if (item.Version == dbVersion.Value)
                    {
                        foreach (var d in item.Templates)
                        {
                            var dt = await context.DocumentTypes.Where(x => x.Name == d.Name).FirstOrDefaultAsync();
                            if (dt == null)
                            {
                                dt = new DocumentType()
                                {
                                    Name = d.Name,
                                    Description = d.Description,
                                    IsSystemType = d.IsSystemType
                                };
                                context.DocumentTypes.Add(dt);
                                await context.SaveChangesAsync();
                                context.Entry(dt).State = EntityState.Detached;
                            }
                        }
                    }
                }

                // Manufacturers
                foreach (var item in ManufacturerCatalog.Catalog)
                {
                    if (item.Version == dbVersion.Value)
                    {
                        foreach (var m in item.Templates)
                        {
                            var md = await context.Manufacturers.Where(x => x.Name == m.Name).FirstOrDefaultAsync();
                            if (md == null)
                            {
                                md = new Manufacturer()
                                {
                                    Name = m.Name
                                };
                                context.Manufacturers.Add(md);
                                await context.SaveChangesAsync();
                                context.Entry(md).State = EntityState.Detached;
                            }
                        }
                    }
                }


                //Asset types
                foreach (var item in AssetTypeCatalog.Catalog)
                {
                    if (item.Version == dbVersion.Value)
                    {
                        foreach (var a in item.Templates)
                        {
                            var at = await context.AssetTypes.Where(x => x.Name == a.Name).FirstOrDefaultAsync();
                            if (at == null)
                            {
                                at = new AssetType()
                                {
                                    Name = a.Name,
                                    Description = a.Description
                                };
                                context.AssetTypes.Add(at);
                                await context.SaveChangesAsync();
                                context.Entry(at).State = EntityState.Detached;
                            }
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
                    ConfigOptConsts.System_UploadPath,
                    ConfigOptConsts.System_SaveUploadsToDatabase,
                    ConfigOptConsts.System_UploadFileSizeLimit,
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
                        "Allows creating a new Role. Requires RolesViewClaims, RolesEditClaims."),
                    new DefaultRoleClaim.NewRoleClaim(ClaimConstantStrings.RolesViewClaims,
                        "Allows viewing a Role's claims."),
                    new DefaultRoleClaim.NewRoleClaim(ClaimConstantStrings.RolesEditClaims,
                        "Allows Editing a Role's claims."),
                    new DefaultRoleClaim.NewRoleClaim(ClaimConstantStrings.RolesDeleteRole,
                        "Allows deleting a role."),
                    new DefaultRoleClaim.NewRoleClaim(ClaimConstantStrings.LicenseTypeAdmin,
                        "Allows creating, editing, and removing License Types."),
                    new DefaultRoleClaim.NewRoleClaim(ClaimConstantStrings.GroupAdmin,
                        "Allows creating, editing, and removing groups."),
                    new DefaultRoleClaim.NewRoleClaim(ClaimConstantStrings.SitewideConfigurationEditor,
                        "Allows editing sitewide configuration settings."),
                    new DefaultRoleClaim.NewRoleClaim(ClaimConstantStrings.UsersAdmin,
                        "Allows creating users, resetting passwords for users, enabling/disabling users."),
                    new DefaultRoleClaim.NewRoleClaim(ClaimConstantStrings.UsersRolesAdmin,
                        "Allows granting/revoking roles for users. Requires UsersAdmin to get to the page to do this."),
                    new DefaultRoleClaim.NewRoleClaim(ClaimConstantStrings.UsersAllowReadAccess,
                        "Required to access the users screen and view basic user details."),
                    new DefaultRoleClaim.NewRoleClaim(ClaimConstantStrings.UsersAllowReadLicenseProductCode,
                        "Allows viewing user license product codes on user details page."),
                    new DefaultRoleClaim.NewRoleClaim(ClaimConstantStrings.ImportExport,
                        "Import and Export data from the site."),
                    new DefaultRoleClaim.NewRoleClaim(ClaimConstantStrings.AssetOptionsEditor,
                        "Allows creating/editing/removing asset options like Asset Types, Manufacturers, Models, Vendors."),
                    new DefaultRoleClaim.NewRoleClaim(ClaimConstantStrings.AssetsManager,
                        "Allows editing and creating assets, setting properties, assigning to users, and assigning licenses."),
                    new DefaultRoleClaim.NewRoleClaim(ClaimConstantStrings.AssetsAllowReadAccess,
                        "Required to access the assets screen and view basic asset details. "),
                    new DefaultRoleClaim.NewRoleClaim(ClaimConstantStrings.AssetsAllowReadLicenseProductCode,
                        "Allows viewing license product codes for assets on the details page."),
                    new DefaultRoleClaim.NewRoleClaim(ClaimConstantStrings.FileManagerAdminAccess,
                        "Allows uploading, downloading, or deleting uploaded files using File Manager."),
                    new DefaultRoleClaim.NewRoleClaim(ClaimConstantStrings.DocumentTypeAdmin,
                        "Allows creating, editing, or deleting document types for File Uplaods."),


                }
            },
            new DefaultRoleClaim()
            {
                Version = string.Empty,
                RoleName = RoleConstantStrings.UserAdmin,
                RoleDescription = "User Admins can edit and create users, enable/disable accounts, and assign licenses.",
                Claims = new List<DefaultRoleClaim.NewRoleClaim>()
                {
                    new DefaultRoleClaim.NewRoleClaim(ClaimConstantStrings.UsersAdmin,
                        "Allows creating users, resetting passwords for users, enabling/disabling users."),
                    new DefaultRoleClaim.NewRoleClaim(ClaimConstantStrings.UsersRolesAdmin,
                        "Allows granting/revoking roles for users. Requires UsersAdmin to get to the page to do this."),
                    new DefaultRoleClaim.NewRoleClaim(ClaimConstantStrings.UsersAllowReadAccess,
                        "Required to access the users screen and view basic user details."),
                    new DefaultRoleClaim.NewRoleClaim(ClaimConstantStrings.UsersAllowReadLicenseProductCode,
                        "Allows viewing user license product codes on user details page."),
                }
            },
            new DefaultRoleClaim()
            {
                Version = string.Empty,
                RoleName = RoleConstantStrings.UserReviewer,
                RoleDescription = "Grants readonly access to users to view properties and assignments.",
                Claims = new List<DefaultRoleClaim.NewRoleClaim>()
                {
                    new DefaultRoleClaim.NewRoleClaim(ClaimConstantStrings.UsersAllowReadAccess,
                        "Required to access the users screen and view basic user details."),
                }
            },
            new DefaultRoleClaim()
            {
                Version = string.Empty,
                RoleName = RoleConstantStrings.AssetAdmin,
                RoleDescription = "Asset Admins can edit, create, and delete assets, assign assets to users, and assign licenses.",
                Claims = new List<DefaultRoleClaim.NewRoleClaim>()
                {
                    new DefaultRoleClaim.NewRoleClaim(ClaimConstantStrings.AssetOptionsEditor,
                        "Allows creating/editing/removing asset options like Asset Types, Manufacturers, Models, Vendors."),
                    new DefaultRoleClaim.NewRoleClaim(ClaimConstantStrings.AssetsManager,
                        "Allows editing, deleting, and creating assets, setting properties, assigning to users, and assigning licenses."),
                    new DefaultRoleClaim.NewRoleClaim(ClaimConstantStrings.AssetsAllowReadAccess,
                        "Required to access the assets screen and view basic asset details."),
                    new DefaultRoleClaim.NewRoleClaim(ClaimConstantStrings.AssetsAllowReadLicenseProductCode,
                        "Allows viewing license product codes for assets on the details page."),
                }
            },
            new DefaultRoleClaim()
            {
                Version = string.Empty,
                RoleName = RoleConstantStrings.AssetReviewer,
                RoleDescription = "Grants readonly access to assets to view properties and assignments.",
                Claims = new List<DefaultRoleClaim.NewRoleClaim>()
                {
                    new DefaultRoleClaim.NewRoleClaim(ClaimConstantStrings.AssetsAllowReadAccess,
                        "Required to access the assets screen and view basic asset details."),
                }
            },
            new DefaultRoleClaim()
            {
                Version = string.Empty,
                RoleName = RoleConstantStrings.FileAdmin,
                RoleDescription = "Allows downloading and deleting all files using the File Manager.",
                Claims = new List<DefaultRoleClaim.NewRoleClaim>()
                {
                    new DefaultRoleClaim.NewRoleClaim(ClaimConstantStrings.FileManagerAdminAccess,
                        "Allows uploading, downloading, or deleting uploaded files using File Manager."),
                }
            },
            new DefaultRoleClaim()
            {
                Version = string.Empty,
                RoleName = RoleConstantStrings.FileOwnEditor,
                RoleDescription = "Allows downloading and deleting all files owned by the user in File Manager.",
                Claims = new List<DefaultRoleClaim.NewRoleClaim>()
                {
                    new DefaultRoleClaim.NewRoleClaim(ClaimConstantStrings.FileManagerOwnAccess,
                        "Allows downloading and deleting documents owned by the user in File Manager."),
                }
            },
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
                    LicenseTypeLink = true,
                    GroupsLink = true,
                    ImportExportLink = true,
                    AssetTypesLink = true,
                    ManufacturersLink = true,
                    DocumentTypesLink = true,
                    FileManagerLink = true,
                }
            }
        };
    }

    public class DocumentTypeVersion
    {
        public string Version { get; set; }
        public List<DocumentType> Templates { get; set; }
    }

    public static class DocumentTypeCatalog
    {
        public static List<DocumentTypeVersion> Catalog = new List<DocumentTypeVersion>()
        {
            new DocumentTypeVersion()
            {
                Version = "",
                Templates = new List<DocumentType>()
                {
                    new DocumentType()
                    {
                        Name = DocumentTypeStrings.ImportUsersUpload,
                        Description = "File upload for the Import Users process. Safe to delete when import is done.",
                        IsSystemType = true,
                    },
                    new DocumentType()
                    {
                        Name = DocumentTypeStrings.ImportUsersResults,
                        Description = "Results of the Users Import process. Safe to delete when import is done.",
                        IsSystemType = true,
                    },
                    new DocumentType()
                    {
                        Name = DocumentTypeStrings.ImportAssetsUpload,
                        Description = "File upload for the Import Assets process. Safe to delete when import is done.",
                        IsSystemType = true,
                    },
                    new DocumentType()
                    {
                        Name = DocumentTypeStrings.ImportAssetsResults,
                        Description = "Results of the Assets Import process. Safe to delete when import is done.",
                        IsSystemType = true,
                    },
                    new DocumentType()
                    {
                        Name = "Contract",
                        Description = "A written or spoken agreement, especially one concerning employment, sales, or tenancy, that is intended to be enforceable by law.",
                        IsSystemType = false,
                    },
                    new DocumentType()
                    {
                        Name = "Documentation of bylaws",
                        Description = "A legal document that describes the structure of an organization, such as a corporation or a nonprofit.",
                        IsSystemType = false,
                    },
                    new DocumentType()
                    {
                        Name = "Non-disclosure agreement",
                        Description = "A binding contract between two or more parties that prevents sensitive information from being shared with others.",
                        IsSystemType = false,
                    },
                    new DocumentType()
                    {
                        Name = "Employment agreement",
                        Description = "An employment contract is an agreement between an employer and employee regarding the employee's term of employment.",
                        IsSystemType = false,
                    },
                    new DocumentType()
                    {
                        Name = "Business plan",
                        Description = "A document that defines in detail a company's objectives and how it plans to achieve its goals.",
                        IsSystemType = false,
                    },
                    new DocumentType()
                    {
                        Name = "Balance sheet",
                        Description = "A summary of the financial balances of an individual or organization.",
                        IsSystemType = false,
                    },
                    new DocumentType()
                    {
                        Name = "Income statement",
                        Description = "Presents the financial results of a business for a stated period of time, aggregating all revenues and expenses.",
                        IsSystemType = false,
                    },
                    new DocumentType()
                    {
                        Name = "Cash flow statmenet",
                        Description = "Provides aggregate data regarding all cash inflows a company receives from its ongoing operations and external investment sources.",
                        IsSystemType = false,
                    },
                    new DocumentType()
                    {
                        Name = "Statement of shareholders' equity",
                        Description = "Details the changes within the equity section of the balance sheet over a designated period of time.",
                        IsSystemType = false,
                    },
                    new DocumentType()
                    {
                        Name = "Quotation",
                        Description = "A formal statement setting out the estimated cost for a particular job or service, including quantities, prices and terms.",
                        IsSystemType = false,
                    },
                    new DocumentType()
                    {
                        Name = "Customer Order",
                        Description = "A formal order from the customer which provides details of the amount and due date for a customer's requirement of products.",
                        IsSystemType = false,
                    },
                    new DocumentType()
                    {
                        Name = "Invoice",
                        Description = "A list of goods sent or services provided, with a statement of the sum due for these; a bill.",
                        IsSystemType = false,
                    },
                    new DocumentType()
                    {
                        Name = "Credit Note",
                        Description = "A document issued by a seller to a buyer to notify that credit is being applied to their account.",
                        IsSystemType = false,
                    },
                    new DocumentType()
                    {
                        Name = "Supplier Order",
                        Description = "A document generated by the buyer and serves the purpose of ordering goods from the supplier.",
                        IsSystemType = false,
                    },
                    new DocumentType()
                    {
                        Name = "Compliance and regulatory document",
                        Description = "Documents or information including records, reports, observations and responses required to verify compliance with standards by a facility or program.",
                        IsSystemType = false,
                    },
                    new DocumentType()
                    {
                        Name = "Business report",
                        Description = "Document that helps the organization's management gain insight into various internal aspects.",
                        IsSystemType = false,
                    },
                    new DocumentType()
                    {
                        Name = "Minutes of business meeting",
                        Description = "An instant written record of a meeting or hearing.",
                        IsSystemType = false,
                    },
                    new DocumentType()
                    {
                        Name = "Contractor agreement",
                        Description = "A contract between a freelancer and a company or client outlining the specifics of their work together.",
                        IsSystemType = false,
                    },
                    new DocumentType()
                    {
                        Name = "Business insurance policy",
                        Description = "A contract of insurance between the insurance company and the policyholder containing the key features, terms and conditions.",
                        IsSystemType = false,
                    },
                    new DocumentType()
                    {
                        Name = "Partnership agreement",
                        Description = "A legal document that dictates how a small for-profit business will operate under two or more people.",
                        IsSystemType = false,
                    },
                    new DocumentType()
                    {
                        Name = "Company policy",
                        Description = "A guideline and rulebook for employers to establish formal expectations and standards for employee health and safety, accountability, best practices and processes within a company.",
                        IsSystemType = false,
                    },
                    new DocumentType()
                    {
                        Name = "Franchise agreement",
                        Description = "Legal agreement that creates a franchise relationship between franchisor and franchisee.",
                        IsSystemType = false,
                    },
                    new DocumentType()
                    {
                        Name = "Board resolution",
                        Description = "A formal document that solidifies in writing important decisions that boards of directors make.",
                        IsSystemType = false,
                    },
                    new DocumentType()
                    {
                        Name = "Terms of use",
                        Description = "Rules, specifications, and requirements for the use of a product or service.",
                        IsSystemType = false,
                    },
                    new DocumentType()
                    {
                        Name = "Business pitch deck",
                        Description = "A presentation that new companies usually create that outlines the organization's main characteristics, qualities and aspirations.",
                        IsSystemType = false,
                    },
                    new DocumentType()
                    {
                        Name = "Business license",
                        Description = "Permit issued by a government agency that allow individuals or companies to conduct business within the government's geographical jurisdiction.",
                        IsSystemType = false,
                    },
                    new DocumentType()
                    {
                        Name = "Receipt",
                        Description = "A document confirming the details of a transaction, such as goods or services received, and amount paid.",
                        IsSystemType = false,
                    }
                }
            }
        };
    }

    public class GroupTemplateVerison
    {
        public string Version { get; set; }
        public List<Group> Groups { get; set; }
    }

    public static class GroupCatalog
    {
        public static List<GroupTemplateVerison> Catalog = new List<GroupTemplateVerison>()
        {
            new GroupTemplateVerison()
            {
                Version = "",
                Groups = new List<Group>()
                {
                    new Group()
                    {
                        Name = "Marketing",
                        Description = "Coordinates and produces all materials representing the organization"
                    },
                    new Group()
                    {
                        Name = "Finance",
                        Description = "Obtains and handles all monies on behalf of the organization"
                    },
                    new Group()
                    {
                        Name = "Operations Management",
                        Description = "Administers all business practices in the organization"
                    },
                    new Group()
                    {
                        Name = "Human Resources",
                        Description = "Recruits, hires, fires, and handles benefits for all employees in the organization"
                    },
                    new Group()
                    {
                        Name = "Information Technology",
                        Description = "Oversees the installation and maintenance of computer network systems"
                    }
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
                        IsUserLicense = true,
                        Status = LicenseStatuses.Active
                    },
                    new LicenseType()
                    {
                        Name = "Office 365 G3 GCC",
                        Description = "Office Desktop Products, Email, SharePoint, and Teams for Government Community Cloud",
                        Seats = null,
                        DeviceRequireProductCode = false,
                        UserRequireProductCode = false,
                        IsDeviceLicense = false,
                        IsUserLicense = true,
                        Status = LicenseStatuses.Active
                    },
                    new LicenseType()
                    {
                        Name = "Microsoft 365 Audio Conferencing for GCC",
                        Description = "Teams Audio Conferencing license add-on for Office 365 G3 GCC",
                        Seats = null,
                        DeviceRequireProductCode = false,
                        UserRequireProductCode = false,
                        IsDeviceLicense = false,
                        IsUserLicense = true,
                        Status = LicenseStatuses.Active
                    },
                    new LicenseType()
                    {
                        Name = "Project Plan 3 for GCC",
                        Description = "Microsoft Project Online desktop software for Government Community Cloud",
                        Seats = null,
                        DeviceRequireProductCode = false,
                        UserRequireProductCode = false,
                        IsDeviceLicense = false,
                        IsUserLicense = true,
                        Status = LicenseStatuses.Active
                    },
                    new LicenseType()
                    {
                        Name = "Visio Plan 2 for GCC",
                        Description = "Microsoft Visio desktop software for Government Community Cloud",
                        Seats = null,
                        DeviceRequireProductCode = false,
                        UserRequireProductCode = false,
                        IsDeviceLicense = false,
                        IsUserLicense = true,
                        Status = LicenseStatuses.Active
                    },
                    new LicenseType()
                    {
                        Name = "Adobe Creative Cloud",
                        Description = "Adobe Photoshop, Acrobat Pro, Premiere Pro, Illustrator, Lightroom, and InDesign",
                        Seats = null,
                        DeviceRequireProductCode = false,
                        UserRequireProductCode = false,
                        IsDeviceLicense = false,
                        IsUserLicense = true,
                        Status = LicenseStatuses.Active
                    },
                    new LicenseType()
                    {
                        Name = "Adobe Creative Cloud",
                        Description = "Adobe Acrobat Pro DC desktop software",
                        Seats = null,
                        DeviceRequireProductCode = false,
                        UserRequireProductCode = false,
                        IsDeviceLicense = false,
                        IsUserLicense = true,
                        Status = LicenseStatuses.Active
                    },
                    new LicenseType()
                    {
                        Name = "Smartsheet",
                        Description = "Project management software",
                        Seats = null,
                        DeviceRequireProductCode = false,
                        UserRequireProductCode = false,
                        IsDeviceLicense = false,
                        IsUserLicense = true,
                        Status = LicenseStatuses.Active
                    },
                    new LicenseType()
                    {
                        Name = "Trend Micro Worry-Free Business",
                        Description = "Antivirus and endpoint protection software",
                        Seats = null,
                        DeviceRequireProductCode = false,
                        UserRequireProductCode = false,
                        IsDeviceLicense = true,
                        IsUserLicense = false,
                        Status = LicenseStatuses.Active
                    },
                    new LicenseType()
                    {
                        Name = "Avast Business Pro",
                        Description = "Antivirus and endpoint protection software",
                        Seats = null,
                        DeviceRequireProductCode = false,
                        UserRequireProductCode = false,
                        IsDeviceLicense = true,
                        IsUserLicense = false,
                        Status = LicenseStatuses.Active
                    },
                    new LicenseType()
                    {
                        Name = "Windows 10 Professional Upgrade",
                        Description = "Upgrade license for Windows 10 Home to Professional",
                        Seats = null,
                        DeviceRequireProductCode = true,
                        UserRequireProductCode = false,
                        IsDeviceLicense = true,
                        IsUserLicense = false,
                        Status = LicenseStatuses.Active
                    },
                    new LicenseType()
                    {
                        Name = "Windows 11 Professional Upgrade",
                        Description = "Upgrade license for Windows 11 Home to Professional",
                        Seats = null,
                        DeviceRequireProductCode = true,
                        UserRequireProductCode = false,
                        IsDeviceLicense = true,
                        IsUserLicense = false,
                        Status = LicenseStatuses.Active
                    },
                    new LicenseType()
                    {
                        Name = "Windows Server 2019 Standard Volume License",
                        Description = "Server operating system",
                        Seats = null,
                        DeviceRequireProductCode = true,
                        UserRequireProductCode = false,
                        IsDeviceLicense = true,
                        IsUserLicense = false,
                        Status = LicenseStatuses.Active
                    },
                    new LicenseType()
                    {
                        Name = "Windows Server 2019 Datacenter Volume License",
                        Description = "Server operating system",
                        Seats = null,
                        DeviceRequireProductCode = true,
                        UserRequireProductCode = false,
                        IsDeviceLicense = true,
                        IsUserLicense = false,
                        Status = LicenseStatuses.Active
                    }
                }
            }
        };
    }

    public class ManufacturerVersion
    {
        public string Version { get; set; }
        public List<Manufacturer> Templates { get; set; }
    }

    public static class ManufacturerCatalog
    {
        public static List<ManufacturerVersion> Catalog = new List<ManufacturerVersion>()
        {
            new ManufacturerVersion()
            {
                Version = string.Empty,
                Templates = new List<Manufacturer>()
                {
                    new Manufacturer()
                    {
                        Name = "Acer",
                    },
                    new Manufacturer()
                    {
                        Name = "Adobe",
                    },
                    new Manufacturer()
                    {
                        Name = "AMD",
                    },
                    new Manufacturer()
                    {
                        Name = "Apple",
                    },
                    new Manufacturer()
                    {
                        Name = "Asus",
                    },
                    new Manufacturer()
                    {
                        Name = "Broadcom",
                    },
                    new Manufacturer()
                    {
                        Name = "Brother",
                    },
                    new Manufacturer()
                    {
                        Name = "Canon",
                    },
                    new Manufacturer()
                    {
                        Name = "Cisco",
                    },
                    new Manufacturer()
                    {
                        Name = "Dell",
                    },
                    new Manufacturer()
                    {
                        Name = "Dymo",
                    },
                    new Manufacturer()
                    {
                        Name = "Epson",
                    },
                    new Manufacturer()
                    {
                        Name = "Foxconn",
                    },
                    new Manufacturer()
                    {
                        Name = "Framework",
                    },
                    new Manufacturer()
                    {
                        Name = "Fujitsu",
                    },
                    new Manufacturer()
                    {
                        Name = "Google",
                    },
                    new Manufacturer()
                    {
                        Name = "HP",
                    },
                    new Manufacturer()
                    {
                        Name = "Huawei",
                    },
                    new Manufacturer()
                    {
                        Name = "IBM",
                    },
                    new Manufacturer()
                    {
                        Name = "Intel",
                    },
                    new Manufacturer()
                    {
                        Name = "Kyocera",
                    },
                    new Manufacturer()
                    {
                        Name = "Lenovo",
                    },
                    new Manufacturer()
                    {
                        Name = "Lexmark",
                    },
                    new Manufacturer()
                    {
                        Name = "LG",
                    },
                    new Manufacturer()
                    {
                        Name = "Microsoft",
                    },
                    new Manufacturer()
                    {
                        Name = "Motorola",
                    },
                    new Manufacturer()
                    {
                        Name = "NCR",
                    },
                    new Manufacturer()
                    {
                        Name = "NEC",
                    },
                    new Manufacturer()
                    {
                        Name = "Netgear",
                    },
                    new Manufacturer()
                    {
                        Name = "Nintendo",
                    },
                    new Manufacturer()
                    {
                        Name = "Nokia",
                    },
                    new Manufacturer()
                    {
                        Name = "Nvidia",
                    },
                    new Manufacturer()
                    {
                        Name = "Panasonic",
                    },
                    new Manufacturer()
                    {
                        Name = "Pitney Bowes",
                    },
                    new Manufacturer()
                    {
                        Name = "Qualcomm",
                    },
                    new Manufacturer()
                    {
                        Name = "Ricoh",
                    },
                    new Manufacturer()
                    {
                        Name = "Samsung",
                    },
                    new Manufacturer()
                    {
                        Name = "Schneider Electric",
                    },
                    new Manufacturer()
                    {
                        Name = "Sharp",
                    },
                    new Manufacturer()
                    {
                        Name = "Sony",
                    },
                    new Manufacturer()
                    {
                        Name = "Supermicro",
                    },
                    new Manufacturer()
                    {
                        Name = "System76",
                    },
                    new Manufacturer()
                    {
                        Name = "Tektronix",
                    },
                    new Manufacturer()
                    {
                        Name = "Texas Instruments",
                    },
                    new Manufacturer()
                    {
                        Name = "Toshiba",
                    },
                    new Manufacturer()
                    {
                        Name = "Viewsonic",
                    },
                    new Manufacturer()
                    {
                        Name = "VMWare",
                    },
                    new Manufacturer()
                    {
                        Name = "Xerox",
                    },
                    new Manufacturer()
                    {
                        Name = "Xiaomi",
                    },
                    new Manufacturer()
                    {
                        Name = "Zebra",
                    },
                }
            }
        };
    }

    public class AssetTypeVersion
    {
        public string Version { get; set; }
        public List<AssetType> Templates { get; set; }
    }

    public static class AssetTypeCatalog
    {
        public static List<AssetTypeVersion> Catalog = new List<AssetTypeVersion>()
        {
            new AssetTypeVersion()
            {
                Version = "",
                Templates = new List<AssetType>()
                {
                    new AssetType()
                    {
                        Name = "Laptop",
                        Description = "Portable laptops, 2-in-1s, convertables, and foldables."
                    },
                    new AssetType()
                    {
                        Name = "Desktop",
                        Description = "Towers, desktops, fixed mini-pcs, etc."
                    },
                    new AssetType()
                    {
                        Name = "Tablet",
                        Description = "Surface tablets, iPads, eReaders, Android tablets, etc."
                    },
                    new AssetType()
                    {
                        Name = "Phone",
                        Description = "Cell phones, desk phones, etc."
                    },
                    new AssetType()
                    {
                        Name = "Monitor",
                        Description = "Computer display device."
                    },
                    new AssetType()
                    {
                        Name = "Dock",
                        Description = "Plug in and socket docks, port replicators, hubs."
                    },
                    new AssetType()
                    {
                        Name = "Keyboard",
                        Description = "Keyboards, 10 key inputs, and keyboard/mouse combo sets."
                    },
                    new AssetType()
                    {
                        Name = "Mouse",
                        Description = "Wired or wireless mouse, touch pads, track balls, etc."
                    },
                    new AssetType()
                    {
                        Name = "Printer",
                        Description = "Printers, multi-function printers, label makers, receipt printers, etc."
                    },
                    new AssetType()
                    {
                        Name = "Scanner",
                        Description = "Document scanners, flat bed scaners, barcode readers, id readers, etc."
                    },
                    new AssetType()
                    {
                        Name = "Headset",
                        Description = "Headphones, phone headsets, VR headsets, etc."
                    },
                    new AssetType()
                    {
                        Name = "Speaker",
                        Description = "Audio output devices, desktop speakers, conference speakers, etc."
                    },
                    new AssetType()
                    {
                        Name = "Camera",
                        Description = "Still or video camera."
                    },
                    new AssetType()
                    {
                        Name = "Charger",
                        Description = "Removable chargers, laptop and phone chargers, etc."
                    },
                    new AssetType()
                    {
                        Name = "Battery",
                        Description = "Battery or removable power unit, like a spare laptop battery."
                    },
                    new AssetType()
                    {
                        Name = "Server",
                        Description = "Tower or Rack server, virtual machine, etc."
                    },
                    new AssetType()
                    {
                        Name = "Network Device",
                        Description = "Router, firewall, Wi-Fi Access Point, Gateway, Switch, Bridge, etc."
                    }
                }
            }
        };
    }

}
