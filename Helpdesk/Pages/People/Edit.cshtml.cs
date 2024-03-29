﻿using System;
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

            public List<LicenseItem> Licenses { get; set; }
            public List<UserRoleItem> UserRoles { get; set; }

            public List<TeamMbr> Supervisors { get; set; }
            public List<TeamMbr> Subordinates { get; set; }
        }

        public class TeamMbr
        {
            public int Id { get; set; }
            [Display(Name = "Name")]
            public string DisplayName { get; set; }
            public List<TeamMbrResp> Responsibilities { get; set; }
        }

        public class TeamMbrResp
        {
            public int Id { get; set; }
            public string Display { get; set; }
        }

        public class LicenseItem
        {
            public int Id { get; set; }

            public bool Added { get; set; }

            public bool PreviouslyAdded { get; set; }

            public int LicenseTypeId { get; set; }

            public string Name { get; set; } = string.Empty;

            public string Description { get; set; } = string.Empty;

            public bool ShowProductCode { get; set; } = true;
            [Display(Name="Product Code")]
            public string? ProductCode { get; set; } = string.Empty;
        }

        public class UserRoleItem
        {
            public string RoleName { get; set; } = string.Empty;
            public string RoleDescription { get; set; } = string.Empty;
            public bool Added { get; set; }
            public bool PreviouslyAdded { get; set; }
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

            bool ShowUserRoleAdmin = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.UsersRolesAdmin);
            if (ShowUserRoleAdmin)
            {
                ViewData["ShowUserRoleAdmin"] = true;
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
            await PopulateLicenses(iUser);
            await PopulateSupervisors(iUser);
            await PopulateSubordinates(iUser);
            if (ShowUserRoleAdmin)
            {
                await PopulateUserRoles(iUser);
            }
            return Page();
        }
        
        private async Task PopulateUserRoles(IdentityUser iUser)
        {
            var userRoles = await RightsManagement.UserRolesAssigned(_context, iUser.Id);
            var allroles = await RightsManagement.GetAllRoles(_context);
            Input.UserRoles = new List<UserRoleItem>();
            foreach (var r in allroles)
            {
                if (r.IsPrivileged && (_currentIdentityUser == null || !await RightsManagement.UserIsInRole(_context, _currentIdentityUser.Id, r.Name)))
                {
                    if (_currentIdentityUser == null || !await RightsManagement.UserHasSuperAdmin(_context, _currentIdentityUser.Id))
                    {
                        continue;
                    }
                }
                var inRole = userRoles.Where(x => x.Id == r.Id).Any();
                Input.UserRoles.Add(new UserRoleItem()
                {
                    Added = inRole,
                    PreviouslyAdded = inRole,
                    RoleName = r.Name,
                    RoleDescription = r.Description
                });
            }
        }

        private async Task PopulateLicenses(IdentityUser iUser)
        {
            Input.Licenses = new List<LicenseItem>();
            var userLicenses = await _context.UserLicenseAssignments
                .Where(x => x.HelpdeskUser.IdentityUserId == iUser.Id)
                .Include(y => y.LicenseType)
                //.OrderBy(z => z.LicenseType.Name)
                .ToListAsync();
            var availLicenses = await _context.LicenseType
                .Where(x => x.IsUserLicense && x.Status == LicenseStatuses.Active)
                //.OrderBy(y => y.Name)
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
                    PreviouslyAdded = true,
                    ProductCode = ul.ProductCode,
                    ShowProductCode = !string.IsNullOrEmpty(ul.ProductCode) || ul.LicenseType.UserRequireProductCode
                });
            }
            foreach (var al in availLicenses)
            {
                var ul = Input.Licenses.Where(x => x.LicenseTypeId == al.Id).FirstOrDefault();
                if (ul == null)
                {
                    Input.Licenses.Add(new LicenseItem()
                    {
                        Id = -1,
                        LicenseTypeId = al.Id,
                        Name = al.Name,
                        Description = al.Description,
                        Added = false,
                        PreviouslyAdded = false,
                        ProductCode = "",
                        ShowProductCode = al.UserRequireProductCode
                    });
                }
            }
            Input.Licenses = Input.Licenses.OrderBy(x => x.Name).ToList();
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
                await PopulateLicenses(iUser);
                await PopulateSupervisors(iUser);
                await PopulateSubordinates(iUser);
                return NotFound();
            }

            bool ShowUserRoleAdmin = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.UsersRolesAdmin);
            if (ShowUserRoleAdmin)
            {
                ViewData["ShowUserRoleAdmin"] = true;
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
                await PopulateLicenses(iUser);
                await PopulateSupervisors(iUser);
                await PopulateSubordinates(iUser);
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

            // process license selection.
            // It's important to filter database selects to avoid a malicious user specifying an
            // arbitrary user license id and adding/updating/deleting a different user's license
            foreach (var lic in Input.Licenses)
            {
                if (lic.Added && lic.PreviouslyAdded)
                {
                    // check for updated product key
                    var dblic = await _context.UserLicenseAssignments
                        .Where(x => x.Id == lic.Id && x.HelpdeskUser.IdentityUserId == iUser.Id)
                        .Include(y => y.LicenseType)
                        .FirstOrDefaultAsync();
                    if (dblic != null)
                    {
                        if (dblic.ProductCode != lic.ProductCode)
                        {
                            dblic.ProductCode = lic.ProductCode ?? "";
                            _context.UserLicenseAssignments.Update(dblic);
                            await _context.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        // add
                        var dbtype = await _context.LicenseType
                            .Where(x => x.Id == lic.LicenseTypeId)
                            .FirstOrDefaultAsync();
                        if (dbtype != null)
                        {
                            dblic = new UserLicenseAssignment()
                            {
                                HelpdeskUser = hUser,
                                LicenseType = dbtype,
                                ProductCode = lic.ProductCode ?? ""
                            };
                            _context.UserLicenseAssignments.Add(dblic);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
                else if (!lic.Added && lic.PreviouslyAdded)
                {
                    //delete
                    var dellic = await _context.UserLicenseAssignments
                        .Where(x => x.Id == lic.Id && x.HelpdeskUser.IdentityUserId == iUser.Id)
                        .FirstOrDefaultAsync();
                    if (dellic != null)
                    {
                        _context.UserLicenseAssignments.Remove(dellic);
                        await _context.SaveChangesAsync();
                    }
                }
                else if (lic.Added && !lic.PreviouslyAdded)
                {
                    // add
                    var dbtype = await _context.LicenseType
                        .Where(x => x.Id == lic.LicenseTypeId)
                        .FirstOrDefaultAsync();
                    if (dbtype != null)
                    {
                        var addlic = new UserLicenseAssignment()
                        {
                            HelpdeskUser = hUser,
                            LicenseType = dbtype,
                            ProductCode = lic.ProductCode ?? ""
                        };
                        _context.UserLicenseAssignments.Add(addlic);
                        await _context.SaveChangesAsync();
                    }
                }
            }

            var allroles = await RightsManagement.GetAllRoles(_context);
            if (ShowUserRoleAdmin)
            {
                foreach (var r in Input.UserRoles)
                {
                    if (r.Added == r.PreviouslyAdded)
                    {
                        continue;
                    }
                    var role = allroles.Where(x => x.Name == r.RoleName).FirstOrDefault();
                    if (role != null)
                    {
                        // privileged roles can only be added/removed by users that hold those privileges
                        if (role.IsPrivileged && (_currentIdentityUser == null || !await RightsManagement.UserIsInRole(_context, _currentIdentityUser.Id, role.Name)))
                        {
                            // except for SuperAdmins. They can add/remove any privileged role.
                            if (_currentIdentityUser == null || !await RightsManagement.UserHasSuperAdmin(_context, _currentIdentityUser.Id))
                            {
                                continue;
                            }
                        }
                        if (r.Added && !r.PreviouslyAdded)
                        {
                            // add
                            await RightsManagement.UserAddRole(_context, iUser.Id, r.RoleName);
                        }
                        if (!r.Added && r.PreviouslyAdded)
                        {
                            if (role.IsSuperAdmin)
                            {
                                // make sure we aren't about to screw up and remove the last super admin from the site.
                                var rUsers = await RightsManagement.GetUsersWithRole(_context, role.Name);
                                if (rUsers.Count == 1 && rUsers[0].IdentityUserId == iUser.Id)
                                {
                                    // The last super admin can't remove themselves.
                                    continue;
                                }
                            }
                            // remove
                            await RightsManagement.UserRemoveRole(_context, iUser.Id, r.RoleName);
                        }
                    }
                }
            }
            // since we redirect to the HTTPGet method, we don't need to load these here.
            //await PopulateUserRoles(iUser);
            //await PopulateLicenses(iUser);
            return RedirectToPage("/People/Edit", new { id = iUser.Id });
        }


        private async Task PopulateSupervisors(IdentityUser iUser)
        {
            Input.Supervisors = new List<TeamMbr>();
            var team = await _context.TeamMembers
                .Where(x => x.Subordinate.IdentityUserId == iUser.Id)
                .Include(x => x.Supervisor)
                .Include(x => x.Responsibilities)
                .ThenInclude(y => y.Responsibility)
                .ToListAsync();
            foreach (var t in team)
            {
                var teamMember = new TeamMbr();
                teamMember.Id = t.Id;
                teamMember.DisplayName = t.Supervisor.DisplayName;
                teamMember.Responsibilities = new List<TeamMbrResp>();
                foreach (var r in t.Responsibilities)
                {
                    teamMember.Responsibilities.Add(new TeamMbrResp()
                    {
                        Id = r.Id,
                        Display = string.Format("{0} ({1})", r.Responsibility.Name, r.Responsibility.Description)
                    });
                }
                Input.Supervisors.Add(teamMember);
            }
        }

        private async Task PopulateSubordinates(IdentityUser iUser)
        {
            Input.Subordinates = new List<TeamMbr>();
            var team = await _context.TeamMembers
                .Where(x => x.Supervisor.IdentityUserId == iUser.Id)
                .Include(x => x.Subordinate)
                .Include(x => x.Responsibilities)
                .ThenInclude(y => y.Responsibility)
                .ToListAsync();
            foreach (var t in team)
            {
                var teamMember = new TeamMbr();
                teamMember.Id = t.Id;
                teamMember.DisplayName = t.Subordinate.DisplayName;
                teamMember.Responsibilities = new List<TeamMbrResp>();
                foreach (var r in t.Responsibilities)
                {
                    teamMember.Responsibilities.Add(new TeamMbrResp()
                    {
                        Id = r.Id,
                        Display = string.Format("{0} ({1})", r.Responsibility.Name, r.Responsibility.Description)
                    });
                }
                Input.Subordinates.Add(teamMember);
            }
        }
    }
}
