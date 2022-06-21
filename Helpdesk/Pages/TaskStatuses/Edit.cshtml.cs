using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Helpdesk.Data;
using TaskStatus = Helpdesk.Data.TaskStatus;

namespace Helpdesk.Pages.TaskStatuses
{
    public class EditModel : PageModel
    {
        private readonly Helpdesk.Data.ApplicationDbContext _context;

        public EditModel(Helpdesk.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public TaskStatus TaskStatus { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.TaskStatuses == null)
            {
                return NotFound();
            }

            var taskstatus =  await _context.TaskStatuses.FirstOrDefaultAsync(m => m.Id == id);
            if (taskstatus == null)
            {
                return NotFound();
            }
            TaskStatus = taskstatus;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(TaskStatus).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskStatusExists(TaskStatus.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool TaskStatusExists(int id)
        {
          return (_context.TaskStatuses?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
