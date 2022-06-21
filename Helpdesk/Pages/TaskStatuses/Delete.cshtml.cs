using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Helpdesk.Data;
using TaskStatus = Helpdesk.Data.TaskStatus;
using Helpdesk.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Helpdesk.Authorization;

namespace Helpdesk.Pages.TaskStatuses
{
    public class DeleteModel : DI_BasePageModel
    {
        public DeleteModel(ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
            : base(dbContext, userManager, signInManager)
        { }

        [BindProperty]
      public TaskStatus TaskStatus { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            await LoadSiteSettings(ViewData);
            if (_currentHelpdeskUser == null)
            {
                // This happens when a user logs in, but hasn't set up their profile yet.
                return Forbid();
            }
            bool HasClaim = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.TicketOptionsEditor);
            if (!HasClaim)
            {
                return Forbid();
            }
            if (id == null || _context.TaskStatuses == null)
            {
                return NotFound();
            }

            var taskstatus = await _context.TaskStatuses.FirstOrDefaultAsync(m => m.Id == id);

            if (taskstatus == null)
            {
                return NotFound();
            }
            else if (taskstatus.IsSystemType)
            {
                return Forbid();
            }

            if (taskstatus != null)
            {
                bool used = await _context.TicketTasks.Where(x => x.TaskStatus == taskstatus).AnyAsync();
                if (used)
                {
                    ModelState.AddModelError("", "This status has been used and cannot be deleted.");
                }
                TaskStatus = taskstatus;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            await LoadSiteSettings(ViewData);
            if (_currentHelpdeskUser == null)
            {
                // This happens when a user logs in, but hasn't set up their profile yet.
                return Forbid();
            }
            bool HasClaim = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.TicketOptionsEditor);
            if (!HasClaim)
            {
                return Forbid();
            }
            if (id == null || _context.TaskStatuses == null)
            {
                return NotFound();
            }
            var taskstatus = await _context.TaskStatuses.FindAsync(id);

            if (taskstatus != null)
            {
                bool used = await _context.TicketTasks.Where(x => x.TaskStatus == taskstatus).AnyAsync();
                if (used)
                {
                    ModelState.AddModelError("", "This status has been used and cannot be deleted.");
                    return Page();
                }
            }

            if (taskstatus != null)
            {
                TaskStatus = taskstatus;
                _context.TaskStatuses.Remove(TaskStatus);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
