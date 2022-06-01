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

namespace Helpdesk.Pages.People
{
    public class EditModel : DI_BasePageModel
    {
        public EditModel(ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
            : base(dbContext, userManager, signInManager)
        { }

        [BindProperty]
        public InputModel UserModel { get; set; } = default!;

        public class InputModel
        {
            [Required]
            public string Id { get; set; }
            [EmailAddress]
            [Required]
            public string Email { get; set; }
            [Phone]
            public string? PhoneNumber { get; set; }
            [Required]
            public string GivenName { get; set; }
            [Required]
            public string Surname { get; set; }
            public string? JobTitle { get; set; }
            public string? Company { get; set; }
            [Required]
            public bool Enabled { get; set; }
            public bool LockedOut { get; set; }
            public bool EmailVerified { get; set; }
            public bool MFAEnabled { get; set; }
            
        }

        public async Task<IActionResult> OnGetAsync(string? id)
        {
            await LoadSiteSettings(ViewData);
            if (_currentHelpdeskUser == null)
            {
                // This happens when a user logs in, but hasn't set up their profile yet.
                return Forbid();
                // For some pages, it might make sense to redirect to the account profile page so they can immediately enter their details.
                //return RedirectToPage("/Identity/Account/Manage");
            }
            bool HasClaim = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.UsersAdmin);
            if (!HasClaim)
            {
                return Forbid();
            }
            if (id == null || _context.HelpdeskUsers == null)
            {
                return NotFound();
            }



            var helpdeskuser = new HelpdeskUser();  //await _context.HelpdeskUsers.FirstOrDefaultAsync(m => m.Id == id);
            if (helpdeskuser == null)
            {
                return NotFound();
            }
            UserModel = new InputModel();// helpdeskuser;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}

            ////_context.Attach(HelpdeskUser).State = EntityState.Modified;

            //try
            //{
            //    await _context.SaveChangesAsync();
            //}
            //catch (DbUpdateConcurrencyException)
            //{
            //    if (!HelpdeskUserExists(HelpdeskUser.Id))
            //    {
            //        return NotFound();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

            return RedirectToPage("./Index");
        }

        private bool HelpdeskUserExists(int id)
        {
          return (_context.HelpdeskUsers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
