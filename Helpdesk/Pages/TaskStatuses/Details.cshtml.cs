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
    public class DetailsModel : PageModel
    {
        private readonly Helpdesk.Data.ApplicationDbContext _context;

        public DetailsModel(Helpdesk.Data.ApplicationDbContext context)
        {
            _context = context;
        }

      public TaskStatus TaskStatus { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.TaskStatuses == null)
            {
                return NotFound();
            }

            var taskstatus = await _context.TaskStatuses.FirstOrDefaultAsync(m => m.Id == id);
            if (taskstatus == null)
            {
                return NotFound();
            }
            else 
            {
                TaskStatus = taskstatus;
            }
            return Page();
        }
    }
}
