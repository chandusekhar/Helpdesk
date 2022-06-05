using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Helpdesk.Data;
using Helpdesk.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Helpdesk.Authorization;
using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Pages.People
{
    public class DeleteModel : DI_BasePageModel
    {

        public DeleteModel(ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
            : base(dbContext, userManager, signInManager)
        { }

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public class InputModel
        {
            [Required]
            public string Id { get; set; }
            [EmailAddress]
            public string Email { get; set; } = string.Empty;
            [Display(Name = "Given Name")]
            public string GivenName { get; set; } = string.Empty;
            public string Surname { get; set; } = string.Empty;
            [Display(Name = "Display Name")]
            public string DisplayName { get; set; } = string.Empty;
            [Display(Name = "Job Title")]
            public string? JobTitle { get; set; }
            public string? Company { get; set; }
            [Phone]
            [Display(Name = "Phone number")]
            public string? PhoneNumber { get; set; }
            [Display(Name = "Account Enabled")]
            public string Enabled { get; set; }
        }

        public List<string> SiteNavTemplates { get; set; } = new List<string>();
        public List<string> EnabledOptions { get; set; } = new List<string> { "Enabled", "Disabled" };

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

            var iuser = await _userManager.FindByIdAsync(id);
            if (iuser == null)
            {
                return NotFound();
            }
            Input = new InputModel()
            {
                Id = iuser.Id,
            };
            Input.Email = await _userManager.GetEmailAsync(iuser);
            Input.PhoneNumber = await _userManager.GetPhoneNumberAsync(iuser);

            var huser = await _context.HelpdeskUsers.FirstOrDefaultAsync(m => m.IdentityUserId == id);

            if (huser != null)
            {
                Input.GivenName = huser.GivenName;
                Input.Surname = huser.Surname;
                Input.DisplayName = huser.DisplayName;
                Input.JobTitle = huser.JobTitle;
                Input.Company = huser.Company;
                Input.Enabled = huser.IsEnabled ? "Enabled" : "Disabled";
            }

            return Page();
        }

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
            bool HasClaim = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.UsersAdmin);
            if (!HasClaim)
            {
                return Forbid();
            }

            if (_context.HelpdeskUsers == null || _userManager == null)
            {
                return NotFound();
            }

            var iuser = await _userManager.FindByIdAsync(Input.Id);
            if (iuser == null)
            {
                return NotFound();
            }

            var huser = await _context.HelpdeskUsers
                .Where(x => x.IdentityUserId == Input.Id)
                .FirstOrDefaultAsync();

            var lic = await _context.UserLicenseAssignments
                .Where(x => x.HelpdeskUser.IdentityUserId == iuser.Id)
                .ToListAsync();

            _context.UserLicenseAssignments.RemoveRange(lic);
            await _context.SaveChangesAsync();

            if (huser != null)
            { 
                await RightsManagement.RemoveAllRolesFromUser(_context, Input.Id);
                _context.HelpdeskUsers.Remove(huser);
                await _context.SaveChangesAsync();
            }

            await _userManager.DeleteAsync(iuser);
            return RedirectToPage("./Index");
        }
    }
}
