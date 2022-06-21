using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Helpdesk.Data;
using TaskStatus = Helpdesk.Data.TaskStatus;

namespace Helpdesk.Pages.TaskStatuses
{
    public class IndexModel : PageModel
    {
        private readonly Helpdesk.Data.ApplicationDbContext _context;

        public IndexModel(Helpdesk.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<TaskStatus> TaskStatus { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.TaskStatuses != null)
            {
                TaskStatus = await _context.TaskStatuses.ToListAsync();
            }
        }
    }
}
