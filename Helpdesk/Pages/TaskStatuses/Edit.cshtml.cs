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
using Helpdesk.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Helpdesk.Authorization;

namespace Helpdesk.Pages.TaskStatuses
{
    public class EditModel : DI_BasePageModel
    {
        public EditModel(ApplicationDbContext dbContext,
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

            var taskstatus =  await _context.TaskStatuses.FirstOrDefaultAsync(m => m.Id == id);
            if (taskstatus == null)
            {
                return NotFound();
            }
            if (taskstatus.IsSystemType)
            {
                return Forbid();
            }
            TaskStatus = taskstatus;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
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
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var ts = await _context.TaskStatuses.Where(x => x.Id == TaskStatus.Id).FirstOrDefaultAsync();
            if (ts == null)
            {
                return NotFound();
            }
            if (ts.IsSystemType)
            {
                return Forbid();
            }
            if (ts.Name != TaskStatus.Name)
            {
                var tsOld = await _context.TaskStatuses.Where(x => x.Name == TaskStatus.Name).FirstOrDefaultAsync();
                if (tsOld != null)
                {
                    ModelState.AddModelError("TaskStatus.Name", "Status name is already in use.");
                    return Page();
                }
            }
            ts.Name = TaskStatus.Name;
            ts.Description = TaskStatus.Description;
            ts.IsCompleted = TaskStatus.IsCompleted;
            _context.TaskStatuses.Update(ts);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private bool TaskStatusExists(int id)
        {
          return (_context.TaskStatuses?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
