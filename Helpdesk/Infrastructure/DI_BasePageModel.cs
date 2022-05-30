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
        public DI_BasePageModel(ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _context = dbContext;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        protected async Task LoadSiteSettings(ViewDataDictionary viewData)
        {
            ConfigOpt? opt = await _context.ConfigOpts
                .Where(x => x.Category == ConfigOptConsts.Branding_SiteName.Category &&
                            x.Key == ConfigOptConsts.Branding_SiteName.Key)
                .FirstOrDefaultAsync();
            viewData[ViewDataStrings.Brand_SiteName] = opt?.Value ?? "Helpdesk";

            opt = await _context.ConfigOpts
                .Where(x => x.Category == ConfigOptConsts.Branding_OrganizationName.Category &&
                            x.Key == ConfigOptConsts.Branding_OrganizationName.Key)
                .FirstOrDefaultAsync();
            viewData[ViewDataStrings.Brand_OrganizationName] = opt?.Value ?? "Our Organization";

            opt = await _context.ConfigOpts
                .Where(x => x.Category == ConfigOptConsts.Branding_SiteURL.Category &&
                            x.Key == ConfigOptConsts.Branding_SiteURL.Key)
                .FirstOrDefaultAsync();
            viewData[ViewDataStrings.Brand_SiteURL] = opt?.Value ?? "helpdesk.localhost";

            opt = await _context.ConfigOpts
                .Where(x => x.Category == ConfigOptConsts.Accounts_AllowSelfRegistration.Category &&
                            x.Key == ConfigOptConsts.Accounts_AllowSelfRegistration.Key)
               .FirstOrDefaultAsync();
            if ((opt?.Value ?? "true") == "true")
            {
                viewData[ViewDataStrings.Accounts_ShowRegister] = "true";
            }
        }
    }
}
