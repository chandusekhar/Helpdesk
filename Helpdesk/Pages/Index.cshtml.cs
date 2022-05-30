using Helpdesk.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Helpdesk.Pages
{
    [AllowAnonymous]
    public class IndexModel : DI_BasePageModel
    {
        public IndexModel(ApplicationDbContext dbContext,
            ILogger<IndexModel> logger)
            :base(dbContext, logger)
        {
            
        }

        public async Task OnGet()
        {
            await LoadBranding(ViewData);
        }
    }
}