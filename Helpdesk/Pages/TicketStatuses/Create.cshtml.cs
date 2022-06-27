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
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk.Pages.TicketStatuses
{
    public class CreateModel : DI_BasePageModel
    {
        public CreateModel(ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
            : base(dbContext, userManager, signInManager)
        { }

        public class InputModel
        {
            [Required]
            public string Name { get; set; }
            [Required]
            public string Description { get; set; }
            [Required]
            [Display(Name="Complete")]
            public bool IsCompleted { get; set; }
            [Required]
            public bool Archived { get; set; }
            [Required]
            [Display(Name = "Display Order")]
            public int DisplayOrder { get; set; }
        }

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

        [BindProperty]
        public InputModel TicketStatus { get; set; } = default!;
        

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
            if (!ModelState.IsValid || _context.TicketStatuses == null || TicketStatus == null)
            {
                return Page();
            }
            var ts = await _context.TicketStatuses.Where(x => x.Name == TicketStatus.Name).FirstOrDefaultAsync();
            if (ts != null)
            {
                ModelState.AddModelError("TicketStatus.Name", "Status name is already in use.");
                return Page();
            }
            ts = new TicketStatus()
            {
                Name = TicketStatus.Name,
                Description = TicketStatus.Description,
                IsCompleted = TicketStatus.IsCompleted,
                IsSystemType = false,
                Archived = TicketStatus.Archived,
                DisplayOrder = TicketStatus.DisplayOrder
            };
            _context.TicketStatuses.Add(ts);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
