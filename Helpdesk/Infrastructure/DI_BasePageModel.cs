using Helpdesk.Data;
using Helpdesk.Pages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk.Infrastructure
{
    public class DI_BasePageModel : PageModel
    {
        protected readonly ApplicationDbContext _context;
        protected readonly UserManager<IdentityUser> _userManager;
        protected readonly SignInManager<IdentityUser> _signInManager;

        protected IdentityUser? _currentIdentityUser = null;
        protected HelpdeskUser? _currentHelpdeskUser = null;

        private bool LoadSiteSettingsAlreadyDone = false;

        public DI_BasePageModel(ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _context = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        protected async Task<bool> LoadSiteSettings(ViewDataDictionary viewData)
        {
            if (LoadSiteSettingsAlreadyDone)
            {
                return true;
            }
            LoadSiteSettingsAlreadyDone = true;

            // We can build a list of claims the user owns here and prepare some variables for easy use later.
            if (_signInManager.IsSignedIn(User))
            {
                _currentIdentityUser = await _userManager.GetUserAsync(User);
                if (_currentIdentityUser == null)
                {
                    await _signInManager.SignOutAsync();
                }
                else
                {
                    _currentHelpdeskUser = await _context.HelpdeskUsers
                        .Where(x => x.IdentityUserId == _currentIdentityUser.Id)
                        .Include(y => y.SiteNavTemplate)
                        .FirstOrDefaultAsync();
                    if (_currentHelpdeskUser == null)
                    {
                        viewData.Add("IdentityUserName", User.Identity?.Name);
                        viewData.Add("CompleteProfilePrompt", "true");
                    }
                    else
                    {
                        string displayName = _currentHelpdeskUser.DisplayName;
                        if (string.IsNullOrEmpty(displayName))
                        {
                            displayName = _currentHelpdeskUser.GivenName;
                            if (string.IsNullOrEmpty(displayName))
                            {
                                displayName = User.Identity?.Name ?? "Guest";
                            }
                        }
                        viewData.Add("IdentityUserName", displayName);
                        if (!_currentHelpdeskUser.IsEnabled)
                        {
                            await _signInManager.SignOutAsync();
                            return false;
                        }
                        if (!_currentIdentityUser.TwoFactorEnabled)
                        {
                            ConfigOpt? showMfaOpt = await _context.ConfigOpts
                                .Where(x => x.Category == ConfigOptConsts.Accounts_ShowMfaBanner.Category &&
                                            x.Key == ConfigOptConsts.Accounts_ShowMfaBanner.Key)
                                .FirstOrDefaultAsync();
                            if (showMfaOpt?.Value == "true")
                            {
                                viewData.Add("NagMFAEnrollmentBanner", "true");
                            }
                        }
                        // build navbar
                        viewData.Add("NavbarSupressPrivacyLink", "true");
                        if (_currentHelpdeskUser.SiteNavTemplate != null)
                        {
                            if (_currentHelpdeskUser.SiteNavTemplate.TicketLink)
                            {
                                viewData.Add("NavbarShowTicketLink", "true");
                            }
                            if (_currentHelpdeskUser.SiteNavTemplate.AssetLink)
                            {
                                viewData.Add("NavbarShowAssetLink", "true");
                            }
                            if (_currentHelpdeskUser.SiteNavTemplate.PeopleLink)
                            {
                                viewData.Add("NavbarShowPeopleLink", "true");
                            }
                            if (_currentHelpdeskUser.SiteNavTemplate.ShowConfigurationMenu)
                            {
                                viewData.Add("NavbarShowConfigurationMenu", "true");
                            }
                            if (_currentHelpdeskUser.SiteNavTemplate.LicenseTypeLink)
                            {
                                viewData.Add("NavbarShowLicenseTypeLink", "true");
                            }
                            if (_currentHelpdeskUser.SiteNavTemplate.GroupsLink)
                            {
                                viewData.Add("NavbarShowGroupsLink", "true");
                            }
                            if (_currentHelpdeskUser.SiteNavTemplate.SiteSettingsLink)
                            {
                                viewData.Add("NavbarShowSiteSettingsLink", "true");
                            }
                            if (_currentHelpdeskUser.SiteNavTemplate.ImportExportLink)
                            {
                                viewData.Add("NavbarShowImportExportLink", "true");
                            }
                            if (_currentHelpdeskUser.SiteNavTemplate.AssetTypesLink)
                            {
                                viewData.Add("NavbarShowAssetTypesLink", "true");
                            }
                            if (_currentHelpdeskUser.SiteNavTemplate.AssetTypesLink)
                            {
                                viewData.Add("NavbarShowManufacturersLink", "true");
                            }
                            if (_currentHelpdeskUser.SiteNavTemplate.RoleAdminLink)
                            {
                                viewData.Add("NavbarShowRoleAdminLink", "true");
                            }
                            if (_currentHelpdeskUser.SiteNavTemplate.LocationsLink)
                            {
                                viewData.Add("NavbarShowLocationsLink", "true");
                            }
                            if (_currentHelpdeskUser.SiteNavTemplate.ActionStatusesLink)
                            {
                                viewData.Add("NavbarShowActionStatusesLink", "true");
                            }
                            if (_currentHelpdeskUser.SiteNavTemplate.TicketStatusesLink)
                            {
                                viewData.Add("NavbarShowTicketStatusesLink", "true");
                            }
                            if (_currentHelpdeskUser.SiteNavTemplate.DocumentTypesLink)
                            {
                                viewData.Add("NavbarShowDocumentTypesLink", "true");
                            }
                            if (_currentHelpdeskUser.SiteNavTemplate.FileManagerLink)
                            {
                                viewData.Add("NavbarShowFileManagerLink", "true");
                            }
                        }

                    }
                }
            }

            ConfigOpt? opt = await _context.ConfigOpts
                .Where(x => x.Category == ConfigOptConsts.Branding_SiteName.Category &&
                            x.Key == ConfigOptConsts.Branding_SiteName.Key)
                .FirstOrDefaultAsync();
            viewData[ViewDataStrings.Brand_SiteName] = opt?.Value ?? ConfigOptConsts.Branding_SiteName.Value;

            opt = await _context.ConfigOpts
                .Where(x => x.Category == ConfigOptConsts.Branding_OrganizationName.Category &&
                            x.Key == ConfigOptConsts.Branding_OrganizationName.Key)
                .FirstOrDefaultAsync();
            viewData[ViewDataStrings.Brand_OrganizationName] = opt?.Value ?? ConfigOptConsts.Branding_OrganizationName.Value;

            opt = await _context.ConfigOpts
                .Where(x => x.Category == ConfigOptConsts.Branding_SiteURL.Category &&
                            x.Key == ConfigOptConsts.Branding_SiteURL.Key)
                .FirstOrDefaultAsync();
            viewData[ViewDataStrings.Brand_SiteURL] = opt?.Value ?? ConfigOptConsts.Branding_SiteURL.Value;

            opt = await _context.ConfigOpts
                .Where(x => x.Category == ConfigOptConsts.Accounts_AllowSelfRegistration.Category &&
                            x.Key == ConfigOptConsts.Accounts_AllowSelfRegistration.Key)
               .FirstOrDefaultAsync();
            if ((opt?.Value ?? "true") == "true")
            {
                viewData[ViewDataStrings.Accounts_ShowRegister] = "true";
            }

            return true;
        }
    }
}
