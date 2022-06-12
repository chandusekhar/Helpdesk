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

namespace Helpdesk.Pages.Manufacturers
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
            public int? Id { get; set; }
            [Required]
            public string Name { get; set; }
        }

        [BindProperty]
        public InputModel Manufacturer { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            await LoadSiteSettings(ViewData);
            if (_currentHelpdeskUser == null)
            {
                // This happens when a user logs in, but hasn't set up their profile yet.
                return Forbid();
                // For some pages, it might make sense to redirect to the account profile page so they can immediately enter their details.
                //return RedirectToPage("/Identity/Account/Manage");
            }
            bool HasClaim = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.AssetOptionsEditor);
            if (!HasClaim)
            {
                return Forbid();
            }
            if (id == null || _context.Manufacturers == null)
            {
                return NotFound();
            }

            var manufacturer =  await _context.Manufacturers.FirstOrDefaultAsync(m => m.Id == id);
            if (manufacturer == null)
            {
                return NotFound();
            }
            Manufacturer = new InputModel()
            {
                Id = manufacturer.Id,
                Name = manufacturer.Name,
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
                // For some pages, it might make sense to redirect to the account profile page so they can immediately enter their details.
                //return RedirectToPage("/Identity/Account/Manage");
            }
            bool HasClaim = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.AssetOptionsEditor);
            if (!HasClaim)
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var m = await _context.Manufacturers.Where(x => x.Id == Manufacturer.Id).FirstOrDefaultAsync();
            if (m == null)
            {
                return NotFound();
            }
            m.Name = Manufacturer.Name;
            _context.Manufacturers.Update(m);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        //private bool ManufacturerExists(int id)
        //{
        //  return (_context.Manufacturers?.Any(e => e.Id == id)).GetValueOrDefault();
        //}
    }
}
