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
using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Pages.TicketTypes
{
    public class EditModel : DI_BasePageModel
    {
        public EditModel(ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
            : base(dbContext, userManager, signInManager)
        { }

        public class InputModel
        {
            [Key]
            public int Id { get; set; }
            [Required]
            public string Name { get; set; }
            [Required]
            public string Description { get; set; }
            [Required]
            public string Category { get; set; }

            [Display(Name = "Creation Claim")]
            public string? CreationClaim { get; set; }
            [Display(Name = "Edit Claim")]
            public string? EditClaim { get; set; }
            [Display(Name = "View Claim")]
            public string? ViewClaim { get; set; }
        }

        [BindProperty]
        public InputModel TicketType { get; set; } = default!;

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
            if (id == null || _context.TicketTypes == null)
            {
                return NotFound();
            }

            var tickettype =  await _context.TicketTypes.FirstOrDefaultAsync(m => m.Id == id);
            if (tickettype == null)
            {
                return NotFound();
            }
            TicketType = new InputModel()
            {
                Id = tickettype.Id,
                Name = tickettype.Name,
                Description = tickettype.Description,
                Category = tickettype.Category,
                ViewClaim = tickettype.ViewClaim,
                EditClaim = tickettype.EditClaim,
                CreationClaim = tickettype.CreationClaim
            };

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
            
            var et = await _context.TicketTypes.Where(x => x.Id == TicketType.Id).FirstOrDefaultAsync();
            if (et == null)
            {
                return NotFound();
            }
            bool failed = false;
            if (et.Name != TicketType.Name)
            {
                // check to see if it's in use
                var test = await _context.TicketTypes.Where(x => x.Name == TicketType.Name).FirstOrDefaultAsync();
                if (test != null)
                {
                    ModelState.AddModelError("TicketType.Name", "This name is already in use.");
                    failed = true;
                }
            }
            TicketType.ViewClaim = TicketType.ViewClaim?.Trim() ?? "";
            TicketType.EditClaim = TicketType.EditClaim?.Trim() ?? "";
            TicketType.CreationClaim = TicketType.CreationClaim?.Trim() ?? "";

            if (!string.IsNullOrEmpty(TicketType.ViewClaim))
            {
                if (TicketType.ViewClaim != et.ViewClaim)
                {
                    var test = await _context.TicketTypes.Where(x => x.ViewClaim == et.ViewClaim).FirstOrDefaultAsync();
                    if (test != null)
                    {
                        ModelState.AddModelError("TicketType.ViewClaim", "This name is already in use.");
                        failed = true;
                    }
                }
            }
            if (!string.IsNullOrEmpty(TicketType.EditClaim))
            {
                if (TicketType.EditClaim != et.EditClaim)
                {
                    var test = await _context.TicketTypes.Where(x => x.EditClaim == et.EditClaim).FirstOrDefaultAsync();
                    if (test != null)
                    {
                        ModelState.AddModelError("TicketType.EditClaim", "This name is already in use.");
                        failed = true;
                    }
                }
            }
            if (!string.IsNullOrEmpty(TicketType.CreationClaim))
            {
                if (TicketType.CreationClaim != et.CreationClaim)
                {
                    var test = await _context.TicketTypes.Where(x => x.CreationClaim == et.CreationClaim).FirstOrDefaultAsync();
                    if (test != null)
                    {
                        ModelState.AddModelError("TicketType.CreationClaim", "This name is already in use.");
                        failed = true;
                    }
                }
            }

            if (failed)
            {
                return Page();
            }

            et.Name = TicketType.Name;
            et.Description = TicketType.Description;
            et.Category = TicketType.Category;
            et.ViewClaim = TicketType.ViewClaim;
            et.EditClaim = TicketType.EditClaim;
            et.CreationClaim = TicketType.CreationClaim;

            _context.TicketTypes.Update(et);

            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

    }
}
