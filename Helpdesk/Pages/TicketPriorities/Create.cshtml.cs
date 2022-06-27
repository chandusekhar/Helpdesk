using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Helpdesk.Data;
using Helpdesk.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Helpdesk.Authorization;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Pages.TicketPriorities
{
    public class CreateModel : DI_BasePageModel
    {
        public CreateModel(ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
            : base(dbContext, userManager, signInManager)
        { }

        public async Task<IActionResult> OnGet()
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
            return Page();
        }


        public class InputModel
        {
            [Required]
            public string Name { get; set; }
            [Required]
            public string Description { get; set; }
        }

        [BindProperty]
        public InputModel TicketPriority { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
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
            if (!ModelState.IsValid || _context.TicketPriority == null || TicketPriority == null)
            {
                return Page();
            }
            var tp = await _context.TicketPriority.Where(x => x.Name == TicketPriority.Name).FirstOrDefaultAsync();
            if (tp != null)
            {
                ModelState.AddModelError("TicketPriority.Name", "Name is already in use.");
                return Page();
            }
            tp = new TicketPriority()
            {
                Name = TicketPriority.Name,
                Description = TicketPriority.Description,
                IsSystemType = false
            };
            _context.TicketPriority.Add(tp);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
