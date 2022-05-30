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

        protected async Task LoadBranding(ViewDataDictionary viewData)
        {
            ConfigOpt? siteName = await _context.ConfigOpts
                .Where(x => x.Category == ConfigOptConsts.Branding_SiteName.Category &&
                            x.Key == ConfigOptConsts.Branding_SiteName.Key)
                .FirstOrDefaultAsync();
            if (siteName == null)
            {
                siteName = new ConfigOpt()
                {
                    Category = ConfigOptConsts.Branding_SiteName.Category,
                    Key = ConfigOptConsts.Branding_SiteName.Key,
                    Value = "Helpdesk",
                    Order = null
                };
            }
            viewData["SiteName"] = siteName.Value;

        }
    }
}
