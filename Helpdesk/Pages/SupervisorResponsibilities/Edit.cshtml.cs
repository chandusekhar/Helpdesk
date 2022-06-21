using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Helpdesk.Data;
using Helpdesk.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Helpdesk.Authorization;

namespace Helpdesk.Pages.SupervisorResponsibilities
{
    public class EditModel : DI_BasePageModel
    {
        public EditModel(ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
            : base(dbContext, userManager, signInManager)
        { }

        [BindProperty]
        public SupervisorResponsibility SupervisorResponsibility { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            await LoadSiteSettings(ViewData);
            if (_currentHelpdeskUser == null)
            {
                // This happens when a user logs in, but hasn't set up their profile yet.
                return Forbid();
            }
            bool HasClaim = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.SuperRespsAdminAccess);
            if (!HasClaim)
            {
                return Forbid();
            }
            if (id == null || _context.SupervisorResponsibilities == null)
            {
                return NotFound();
            }

            var supervisorresponsibility =  await _context.SupervisorResponsibilities.FirstOrDefaultAsync(m => m.Id == id);
            if (supervisorresponsibility == null)
            {
                return NotFound();
            }
            SupervisorResponsibility = supervisorresponsibility;
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
            bool HasClaim = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.SuperRespsAdminAccess);
            if (!HasClaim)
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var resp = await _context.SupervisorResponsibilities.Where(x => x.Id == SupervisorResponsibility.Id).FirstOrDefaultAsync();
            if (resp == null)
            {
                return NotFound();
            }
            if (resp.Name != SupervisorResponsibility.Name)
            {
                bool exists = await _context.SupervisorResponsibilities.Where(x => x.Name == SupervisorResponsibility.Name).AnyAsync();
                if (exists)
                {
                    ModelState.AddModelError("SupervisorResponsibility.Name", "The name is already in use.");
                    return Page();
                }
            }
            resp.Name = SupervisorResponsibility.Name;
            resp.Description = SupervisorResponsibility.Description;
            _context.Update(resp);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private bool SupervisorResponsibilityExists(int id)
        {
          return (_context.SupervisorResponsibilities?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
