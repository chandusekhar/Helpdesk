using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Helpdesk.Data;
using TaskStatus = Helpdesk.Data.TaskStatus;

namespace Helpdesk.Pages.TaskStatuses
{
    public class CreateModel : PageModel
    {
        private readonly Helpdesk.Data.ApplicationDbContext _context;

        public CreateModel(Helpdesk.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public TaskStatus TaskStatus { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.TaskStatuses == null || TaskStatus == null)
            {
                return Page();
            }

            _context.TaskStatuses.Add(TaskStatus);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
