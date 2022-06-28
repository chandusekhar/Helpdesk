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
                var test = await _context.TicketTypes
                    .Where(x => x.Name == TicketType.Name)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
                if (test != null)
                {
                    ModelState.AddModelError("TicketType.Name", "This name is already in use.");
                    failed = true;
                }
            }
            TicketType.ViewClaim = TicketType.ViewClaim?.Trim() ?? "";
            TicketType.EditClaim = TicketType.EditClaim?.Trim() ?? "";
            TicketType.CreationClaim = TicketType.CreationClaim?.Trim() ?? "";

            string OldViewClaim = et.ViewClaim ?? "";
            string OldEditClaim = et.EditClaim ?? "";
            string OldCreationClaim = et.CreationClaim ?? "";
            bool inUse = false;

            if (!string.IsNullOrEmpty(TicketType.ViewClaim))
            {
                if (TicketType.ViewClaim != et.ViewClaim)
                {
                    inUse = await _context.TicketTypes
                        .Where(x => x.ViewClaim == et.ViewClaim)
                        .AnyAsync();
                    if (inUse)
                    {
                        ModelState.AddModelError("TicketType.ViewClaim", "This name is already in use.");
                        failed = true;
                    }
                    inUse = await _context.HelpdeskClaims
                        .Where(x => x.Name == et.ViewClaim)
                        .AnyAsync();
                    if (inUse)
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
                    inUse = await _context.TicketTypes
                        .Where(x => x.EditClaim == et.EditClaim)
                        .AnyAsync();
                    if (inUse)
                    {
                        ModelState.AddModelError("TicketType.EditClaim", "This name is already in use.");
                        failed = true;
                    }
                    inUse = await _context.HelpdeskClaims
                        .Where(x => x.Name == et.EditClaim)
                        .AnyAsync();
                    if (inUse)
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
                    inUse = await _context.TicketTypes
                        .Where(x => x.CreationClaim == et.CreationClaim)
                        .AnyAsync();
                    if (inUse)
                    {
                        ModelState.AddModelError("TicketType.CreationClaim", "This name is already in use.");
                        failed = true;
                    }
                    inUse = await _context.HelpdeskClaims
                        .Where(x => x.Name == et.CreationClaim)
                        .AnyAsync();
                    if (inUse)
                    {
                        ModelState.AddModelError("TicketType.CreationClaim", "This name is already in use.");
                        failed = true;
                    }
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

            et.Name = TicketType.Name;
            et.Description = TicketType.Description;
            et.Category = TicketType.Category;
            et.ViewClaim = TicketType.ViewClaim;
            et.EditClaim = TicketType.EditClaim;
            et.CreationClaim = TicketType.CreationClaim;

            _context.TicketTypes.Update(et);

            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(et.ViewClaim) && string.IsNullOrEmpty(OldViewClaim))
            {
                // create new
                var viewClaim = new HelpdeskClaim()
                {
                    Name = et.ViewClaim,
                    Description = string.Format("View Ticket Type {0} > {1}", et.Name, et.Description),
                    IsSystemType = false
                };
                _context.HelpdeskClaims.Add(viewClaim);
                await _context.SaveChangesAsync();
            }
            else if (string.IsNullOrEmpty(et.ViewClaim) && !string.IsNullOrEmpty(OldViewClaim))
            {
                // delete
                var viewClaim = await _context.HelpdeskClaims.Where(x => x.Name == OldViewClaim).FirstOrDefaultAsync();
                if (viewClaim != null)
                {
                    _context.HelpdeskClaims.Remove(viewClaim);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                // update
                var viewClaim = await _context.HelpdeskClaims.Where(x => x.Name == OldViewClaim).FirstOrDefaultAsync();
                if (viewClaim == null)
                {
                    // old didn't exist..so make a new one
                    viewClaim = new HelpdeskClaim()
                    {
                        Name = et.ViewClaim,
                        Description = string.Format("View Ticket Type {0} > {1}", et.Name, et.Description),
                        IsSystemType = false
                    };
                    _context.HelpdeskClaims.Add(viewClaim);
                }
                else
                {
                    viewClaim.Name = et.ViewClaim;
                    viewClaim.Description = string.Format("View Ticket Type {0} > {1}", et.Name, et.Description);
                    viewClaim.IsSystemType = false;
                    _context.HelpdeskClaims.Update(viewClaim);
                }
                await _context.SaveChangesAsync();
            }

            if (!string.IsNullOrEmpty(et.EditClaim) && string.IsNullOrEmpty(OldEditClaim))
            {
                // create new
                var editClaim = new HelpdeskClaim()
                {
                    Name = et.EditClaim,
                    Description = string.Format("Edit Ticket Type {0} > {1}", et.Name, et.Description),
                    IsSystemType = false
                };
                _context.HelpdeskClaims.Add(editClaim);
                await _context.SaveChangesAsync();
            }
            else if (string.IsNullOrEmpty(et.EditClaim) && !string.IsNullOrEmpty(OldEditClaim))
            {
                // delete
                var editClaim = await _context.HelpdeskClaims.Where(x => x.Name == OldEditClaim).FirstOrDefaultAsync();
                if (editClaim != null)
                {
                    _context.HelpdeskClaims.Remove(editClaim);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                // update
                var editClaim = await _context.HelpdeskClaims.Where(x => x.Name == OldEditClaim).FirstOrDefaultAsync();
                if (editClaim == null)
                {
                    // old didn't exist..so make a new one
                    editClaim = new HelpdeskClaim()
                    {
                        Name = et.EditClaim,
                        Description = string.Format("Edit Ticket Type {0} > {1}", et.Name, et.Description),
                        IsSystemType = false
                    };
                    _context.HelpdeskClaims.Add(editClaim);
                }
                else
                {
                    editClaim.Name = et.EditClaim;
                    editClaim.Description = string.Format("Edit Ticket Type {0} > {1}", et.Name, et.Description);
                    editClaim.IsSystemType = false;
                    _context.HelpdeskClaims.Update(editClaim);
                }
                await _context.SaveChangesAsync();
            }

            if (!string.IsNullOrEmpty(et.CreationClaim) && string.IsNullOrEmpty(OldCreationClaim))
            {
                // create new
                var createClaim = new HelpdeskClaim()
                {
                    Name = et.CreationClaim,
                    Description = string.Format("Create Ticket Type {0} > {1}", et.Name, et.Description),
                    IsSystemType = false
                };
                _context.HelpdeskClaims.Add(createClaim);
                await _context.SaveChangesAsync();
            }
            else if (string.IsNullOrEmpty(et.CreationClaim) && !string.IsNullOrEmpty(OldCreationClaim))
            {
                // delete
                var createClaim = await _context.HelpdeskClaims.Where(x => x.Name == OldCreationClaim).FirstOrDefaultAsync();
                if (createClaim != null)
                {
                    _context.HelpdeskClaims.Remove(createClaim);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                // update
                var createClaim = await _context.HelpdeskClaims.Where(x => x.Name == OldCreationClaim).FirstOrDefaultAsync();
                if (createClaim == null)
                {
                    // old didn't exist..so make a new one
                    createClaim = new HelpdeskClaim()
                    {
                        Name = et.CreationClaim,
                        Description = string.Format("Create Ticket Type {0} > {1}", et.Name, et.Description),
                        IsSystemType = false
                    };
                    _context.HelpdeskClaims.Add(createClaim);
                }
                else
                {
                    createClaim.Name = et.CreationClaim;
                    createClaim.Description = string.Format("Create Ticket Type {0} > {1}", et.Name, et.Description);
                    createClaim.IsSystemType = false;
                    _context.HelpdeskClaims.Update(createClaim);
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }

    }
}
