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

namespace Helpdesk.Pages.TicketTypes
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
            public string Category { get; set; }

            [Display(Name = "Creation Claim")]
            public string? CreationClaim { get; set; }
            [Display(Name = "Edit Claim")]
            public string? EditClaim { get; set; }
            [Display(Name = "View Claim")]
            public string? ViewClaim { get; set; }
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
        public InputModel TicketType { get; set; } = default!;

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
            if (!ModelState.IsValid || _context.TicketTypes == null || TicketType == null)
            {
                return Page();
            }
            bool failed = false;
            bool inUse = false;
            // check to see if it's in use
            var test = await _context.TicketTypes
                .Where(x => x.Name == TicketType.Name)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            if (test != null)
            {
                ModelState.AddModelError("TicketType.Name", "This name is already in use.");
                failed = true;
            }

            TicketType.ViewClaim = TicketType.ViewClaim?.Trim() ?? "";
            TicketType.EditClaim = TicketType.EditClaim?.Trim() ?? "";
            TicketType.CreationClaim = TicketType.CreationClaim?.Trim() ?? "";

            if (!string.IsNullOrEmpty(TicketType.ViewClaim))
            {
                inUse = await _context.TicketTypes
                    .Where(x => x.ViewClaim == TicketType.ViewClaim)
                    .AnyAsync();
                if (inUse)
                {
                    ModelState.AddModelError("TicketType.ViewClaim", "This name is already in use.");
                    failed = true;
                }
                inUse = await _context.HelpdeskClaims
                    .Where(x => x.Name == TicketType.ViewClaim)
                    .AnyAsync();
                if (inUse)
                {
                    ModelState.AddModelError("TicketType.ViewClaim", "This name is already in use.");
                    failed = true;
                }
            }
            if (!string.IsNullOrEmpty(TicketType.EditClaim))
            {
                inUse = await _context.TicketTypes
                    .Where(x => x.EditClaim == TicketType.EditClaim)
                    .AnyAsync();
                if (inUse)
                {
                    ModelState.AddModelError("TicketType.EditClaim", "This name is already in use.");
                    failed = true;
                }
                inUse = await _context.HelpdeskClaims
                    .Where(x => x.Name == TicketType.EditClaim)
                    .AnyAsync();
                if (inUse)
                {
                    ModelState.AddModelError("TicketType.EditClaim", "This name is already in use.");
                    failed = true;
                }
            }
            if (!string.IsNullOrEmpty(TicketType.CreationClaim))
            {
                inUse = await _context.TicketTypes
                    .Where(x => x.CreationClaim == TicketType.CreationClaim)
                    .AnyAsync();
                if (inUse)
                {
                    ModelState.AddModelError("TicketType.CreationClaim", "This name is already in use.");
                    failed = true;
                }
                inUse = await _context.HelpdeskClaims
                    .Where(x => x.Name == TicketType.CreationClaim)
                    .AnyAsync();
                if (inUse)
                {
                    ModelState.AddModelError("TicketType.CreationClaim", "This name is already in use.");
                    failed = true;
                }
            }

            bool v = false;
            bool e = false;
            bool c = false;
            if (!string.IsNullOrEmpty(TicketType.ViewClaim) && 
                !string.IsNullOrEmpty(TicketType.EditClaim) && 
                TicketType.ViewClaim == TicketType.EditClaim)
            {
                v = true;
                e = true;
                failed = true;
            }
            if (!string.IsNullOrEmpty(TicketType.ViewClaim) && 
                !string.IsNullOrEmpty(TicketType.CreationClaim) && 
                TicketType.ViewClaim == TicketType.CreationClaim)
            {
                v = true;
                c = true;
                failed = true;
            }
            if (!string.IsNullOrEmpty(TicketType.EditClaim) && 
                !string.IsNullOrEmpty(TicketType.CreationClaim) && 
                TicketType.EditClaim == TicketType.CreationClaim)
            {
                e = true;
                c = true;
                failed = true;
            }
            if (c)
            {
                ModelState.AddModelError("TicketType.CreationClaim", "The claim name must be unique.");
            }
            if (e)
            {
                ModelState.AddModelError("TicketType.EditClaim", "The claim name must be unique.");
            }
            if (v)
            {
                ModelState.AddModelError("TicketType.ViewClaim", "The claim name must be unique.");
            }
            

            if (failed)
            {
                return Page();
            }
            var et = new TicketType()
            {
                Name = TicketType.Name,
                Description = TicketType.Description,
                Category = TicketType.Category,
                ViewClaim = TicketType.ViewClaim,
                EditClaim = TicketType.EditClaim,
                CreationClaim = TicketType.CreationClaim
            };
            _context.TicketTypes.Update(et);

            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(et.ViewClaim))
            {
                var viewClaim = new HelpdeskClaim()
                {
                    Name = et.ViewClaim,
                    Description = string.Format("View Ticket Type {0} > {1}", et.Name, et.Description),
                    IsSystemType = false
                };
                _context.HelpdeskClaims.Add(viewClaim);
                await _context.SaveChangesAsync();
            }

            if (!string.IsNullOrEmpty(et.EditClaim))
            {
                var editClaim = new HelpdeskClaim()
                {
                    Name = et.EditClaim,
                    Description = string.Format("Edit Ticket Type {0} > {1}", et.Name, et.Description),
                    IsSystemType = false
                };
                _context.HelpdeskClaims.Add(editClaim);
                await _context.SaveChangesAsync();
            }

            if (!string.IsNullOrEmpty(et.CreationClaim))
            {
                var creationClaim = new HelpdeskClaim()
                {
                    Name = et.CreationClaim,
                    Description = string.Format("Create Ticket Type {0} > {1}", et.Name, et.Description),
                    IsSystemType = false
                };
                _context.HelpdeskClaims.Add(creationClaim);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
