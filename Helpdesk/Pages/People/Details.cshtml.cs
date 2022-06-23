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
using System.ComponentModel.DataAnnotations;
using Helpdesk.Authorization;

namespace Helpdesk.Pages.People
{
    public class DetailsModel : DI_BasePageModel
    {
        public DetailsModel(ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
            : base(dbContext, userManager, signInManager)
        { }

        public InputModel Input { get; set; } = default!;

        public class InputModel
        {
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
            [Display(Name = "Site Nav Template")]
            public string SiteNavTemplateName { get; set; } = string.Empty;
            [Display(Name = "Account Enabled")]
            public string Enabled { get; set; }

            public string? Group { get; set; }

            public List<LicenseItem> Licenses { get; set; }

            public List<TeamMember> Team { get; set; }
        }

        public class TeamMember
        {
            public int Id { get; set; }
            [Display(Name = "Name")]
            public string DisplayName { get; set; }
            public List<string> Responsibilities { get; set; }
        }

        public class LicenseItem
        {
            public int Id { get; set; }
            public bool Added { get; set; }

            public string Name { get; set; } = string.Empty;

            public string Description { get; set; } = string.Empty;
            public int LicenseTypeId { get; set; }

            public bool ShowProductCode { get; set; } = true;
            [Display(Name = "Product Code")]
            public string? ProductCode { get; set; } = string.Empty;
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
            bool HasClaim = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.UsersAllowReadAccess);
            if (!HasClaim)
            {
                return Forbid();
            }
            bool ClaimShowProductCode = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.UsersAllowReadLicenseProductCode);
            if (ClaimShowProductCode)
            {
                ViewData["ShowProductCode"] = true;
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
            await PopulateLicenses(iUser, ClaimShowProductCode);
            await PopulateTeam(iUser);
            return Page();
        }

        private async Task PopulateLicenses(IdentityUser iUser, bool ClaimShowProductCode)
        {
            Input.Licenses = new List<LicenseItem>();
            var userLicenses = await _context.UserLicenseAssignments
                .Where(x => x.HelpdeskUser.IdentityUserId == iUser.Id)
                .Include(y => y.LicenseType)
                .ToListAsync();

            foreach (var ul in userLicenses)
            {
                Input.Licenses.Add(new LicenseItem()
                {
                    Id = ul.Id,
                    LicenseTypeId = ul.LicenseType.Id,
                    Name = ul.LicenseType.Name,
                    Description = ul.LicenseType.Description,
                    Added = true,
                    ProductCode = ClaimShowProductCode ? ul.ProductCode : "",
                    ShowProductCode = ClaimShowProductCode ? (!string.IsNullOrEmpty(ul.ProductCode) || ul.LicenseType.UserRequireProductCode) : false
                });
            }
            Input.Licenses = Input.Licenses.OrderBy(x => x.Name).ToList();
        }

        private async Task PopulateTeam(IdentityUser iUser)
        {
            Input.Team = new List<TeamMember>();
            var resp = await _context.TeamMembers
                .Where(x => x.Supervisor.IdentityUserId == iUser.Id)
                .Include(y => y.Supervisor)
                .Include(z => z.SupervisorResponsibilities)
                .OrderBy(x => x.Subordinate.DisplayName)
                .ToListAsync();
            foreach (var t in resp)
            {
                var teamMember = new TeamMember();
                teamMember.Id = t.Id;
                teamMember.DisplayName = t.Subordinate.DisplayName;
                teamMember.Responsibilities = new List<string>();
                foreach (var r in t.SupervisorResponsibilities)
                {
                    teamMember.Responsibilities.Add(string.Format("{0} ({1})", r.Name, r.Description));
                }
                Input.Team.Add(teamMember);
            }
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
    }
}
