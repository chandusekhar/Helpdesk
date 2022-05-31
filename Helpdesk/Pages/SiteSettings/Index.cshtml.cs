using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Helpdesk.Data;
using Helpdesk.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace Helpdesk.Pages.SiteSettings
{
    public class IndexModel : DI_BasePageModel
    {

        public IndexModel(ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
            : base(dbContext, userManager, signInManager)
        { }

        public IList<ConfigOpt> ConfigOpt { get;set; } = default!;

        public async Task OnGetAsync()
        {
            await LoadSiteSettings(ViewData);
            if (_context.ConfigOpts != null)
            {
                ConfigOpt = await _context.ConfigOpts.OrderBy(x => x.Category).ThenBy(y => y.Order).ToListAsync();
            }
        }
    }
}
