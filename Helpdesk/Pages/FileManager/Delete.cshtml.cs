using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Helpdesk.Data;

namespace Helpdesk.Pages.FileManager
{
    public class DeleteModel : PageModel
    {
        private readonly Helpdesk.Data.ApplicationDbContext _context;

        public DeleteModel(Helpdesk.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
      public FileUpload FileUpload { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null || _context.FileUploads == null)
            {
                return NotFound();
            }

            var fileupload = await _context.FileUploads.FirstOrDefaultAsync(m => m.Id == id);

            if (fileupload == null)
            {
                return NotFound();
            }
            else 
            {
                FileUpload = fileupload;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null || _context.FileUploads == null)
            {
                return NotFound();
            }
            var fileupload = await _context.FileUploads.FindAsync(id);

            if (fileupload != null)
            {
                FileUpload = fileupload;
                _context.FileUploads.Remove(FileUpload);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
