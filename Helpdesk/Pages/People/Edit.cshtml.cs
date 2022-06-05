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
        public InputModel Input { get; set; } = default!;

        public class InputModel
        {
            [Required]
            public string Id { get; set; }
            [EmailAddress]
            [Required]
            public string Email { get; set; } = string.Empty;
            [Required]
            [Display(Name = "Given Name")]
            public string GivenName { get; set; } = string.Empty;
            [Required]
            public string Surname { get; set; } = string.Empty;
            [Required]
            [Display(Name = "Display Name")]
            public string DisplayName { get; set; } = string.Empty;
            [Display(Name = "Job Title")]
            public string? JobTitle { get; set; }
            public string? Company { get; set; }
            [Phone]
            [Display(Name = "Phone number")]
            public string? PhoneNumber { get; set; }
            [Required]
            [Display(Name = "Site Nav Template")]
            public string SiteNavTemplateName { get; set; } = string.Empty;
            [Required]
            [Display(Name = "Account Enabled")]
            public string Enabled { get; set; }

            public string? Group { get; set; }
        }

        public List<string> SiteNavTemplates { get; set; } = new List<string>();
        public List<string> EnabledOptions { get; set; } = new List<string> { "Enabled", "Disabled" };
        public List<string> GroupOptions { get; set; } = new List<string>();

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
            if (id == null || _context.HelpdeskUsers == null || _userManager == null)
            {
                return NotFound();
            }

            var iUser = await _userManager.FindByIdAsync(id);
            if (iUser == null)
            {
                return NotFound();
            }

            await PopulateList();
            var hUser = await _context.HelpdeskUsers
                .Where(x => x.IdentityUserId == id)
                .Include(y => y.SiteNavTemplate)
                .FirstOrDefaultAsync();

            if (hUser == null)
            {
                var defaultTemplateName = await _context.ConfigOpts
                    .Where(x => x.Category == ConfigOptConsts.Accounts_DefaultNavTemplate.Category &&
                                x.Key == ConfigOptConsts.Accounts_DefaultNavTemplate.Key)
                    .FirstOrDefaultAsync();
                string tempName = defaultTemplateName?.Value ?? ConfigOptConsts.Accounts_DefaultNavTemplate.Value;
                var template = await _context.SiteNavTemplates
                    .Where(x => x.Name == tempName)
                    .FirstOrDefaultAsync();
                hUser = new HelpdeskUser()
                {
                    IdentityUserId = id,
                    SiteNavTemplate = template ?? new SiteNavTemplate { Name = "" },
                    IsEnabled = true
                };
                return NotFound();
            }
            var phoneNumber = await _userManager.GetPhoneNumberAsync(iUser);
            Input = new InputModel
            {
                Id = iUser.Id,
                Email = iUser.Email,
                GivenName = hUser.GivenName,
                Surname = hUser.Surname,
                DisplayName = hUser.DisplayName,
                JobTitle = hUser.JobTitle,
                Company = hUser.Company,
                PhoneNumber = phoneNumber,
                SiteNavTemplateName = hUser.SiteNavTemplate.Name,
                Enabled = hUser.IsEnabled ? "Enable" : "Disable",
                Group = hUser.Group?.Name
            };
            return Page();
        }
        
        private async Task PopulateList()
        {
            SiteNavTemplates = await _context.SiteNavTemplates.Select(x => x.Name).ToListAsync();
            var groups = await _context.Groups.ToListAsync();
            GroupOptions = new List<string>();
            GroupOptions.Add("");
            foreach (var g in groups)
            {
                GroupOptions.Add(g.Name);
            }
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
            if (!ModelState.IsValid || _context.HelpdeskUsers == null || Input == null)
            {
                return Page();
            }

            await PopulateList();
            bool failValidation = false;
            // Find existing user objects
            var iUser = await _userManager.FindByIdAsync(Input.Id);
            if (iUser == null)
            {
                return NotFound();
            }

            // Make sure nav template selection is valid
            var navTemplate = await _context.SiteNavTemplates
                .Where(x => x.Name == Input.SiteNavTemplateName)
                .FirstOrDefaultAsync();
            if (navTemplate == null)
            {
                ModelState.AddModelError("Input.SiteNavTemplateName", "Select a valid Sate Nav Template.");
                failValidation = true;
            }

            // see if user email address is changing
            var email = await _userManager.GetEmailAsync(iUser);
            if (Input.Email != email)
            {
                // See if this email address is taken
                if ((await _userManager.FindByEmailAsync(Input.Email)) != null)
                {
                    ModelState.AddModelError("Input.Email", "That email address is already taken.");
                    failValidation = true;
                }
            }

            if (failValidation)
            {
                return Page();
            }

            // set email
            if (Input.Email != email)
                await _userManager.SetEmailAsync(iUser, Input.Email);

            // set phone number
            await _userManager.SetPhoneNumberAsync(iUser, Input.PhoneNumber);

            var hUser = await _context.HelpdeskUsers
                .Where(x => x.IdentityUserId == iUser.Id)
                .FirstOrDefaultAsync();

            bool enabled = Input.Enabled == "Enabled";

            var selectedgroup = await _context.Groups
                .Where(x => x.Name == Input.Group)
                .FirstOrDefaultAsync();

            if (hUser == null)
            {
                // we're creating a new user object.
                hUser = new HelpdeskUser()
                {
                    IdentityUserId = iUser.Id,
                    IsEnabled = enabled,
                    GivenName = Input.GivenName,
                    Surname = Input.Surname,
                    DisplayName = Input.DisplayName,
                    JobTitle = Input.JobTitle,
                    Company = Input.Company,
                    SiteNavTemplate = navTemplate,
                    Group = selectedgroup
                };
                _context.HelpdeskUsers.Add(hUser);
            }
            else
            {
                hUser.IsEnabled = enabled;
                hUser.GivenName = Input.GivenName;
                hUser.Surname = Input.Surname;
                hUser.DisplayName = Input.DisplayName;
                hUser.JobTitle = Input.JobTitle;
                hUser.Company = Input.Company;
                hUser.SiteNavTemplate = navTemplate;
                hUser.Group = selectedgroup;
                _context.HelpdeskUsers.Update(hUser);
            }
            await _context.SaveChangesAsync();
            return Page();
        }

    }
}
