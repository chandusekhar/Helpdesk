using Helpdesk.Data;
using Helpdesk.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk.Pages
{
    public class DI_BasePageModel : PageModel
    {
        protected readonly ApplicationDbContext _context;
        private readonly ILogger<IndexModel> _logger;
        public DI_BasePageModel(ApplicationDbContext dbContext, 
            ILogger<IndexModel> logger)
        {
            _context = dbContext;
            _logger = logger;
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
