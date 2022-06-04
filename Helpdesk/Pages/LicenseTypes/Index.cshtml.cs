using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Helpdesk.Data;

namespace Helpdesk.Pages.LicenseTypes
{
    public class IndexModel : PageModel
    {
        private readonly Helpdesk.Data.ApplicationDbContext _context;

        public IndexModel(Helpdesk.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<LicenseType> LicenseType { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.LicenseType != null)
            {
                LicenseType = await _context.LicenseType.ToListAsync();
            }
        }
    }
}
