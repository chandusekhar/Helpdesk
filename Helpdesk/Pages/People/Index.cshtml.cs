using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Helpdesk.Data;
using System.ComponentModel.DataAnnotations;
using Helpdesk.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Helpdesk.Pages.People
{
    public class IndexModel : Infrastructure.DI_BasePageModel
    {

        public IndexModel(ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
            : base(dbContext, userManager, signInManager)
        { }

        public IList<InputModel> UserList { get;set; } = default!;

        public class InputModel
        {
            [Required]
            public string Id { get; set; }
            [EmailAddress]
            [Required]
            public string Email { get; set; } = string.Empty!;
            [Display(Name ="Given Name")]
            [Required]
            public string GivenName { get; set; } = string.Empty!;
            [Required]
            public string Surname { get; set; } = string.Empty!;
            [Display(Name ="Display Name")]
            public string DisplayName { get; set; } = string.Empty!;
            [Display(Name ="Job Title")]
            public string JobTitle { get; set; } = string.Empty!;
            public string Company { get; set; } = string.Empty!;
            [Required]
            public bool Enabled { get; set; }
            public string Group { get; set; } = string.Empty!;
        }

        public async Task<IActionResult> OnGetAsync()
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
            bool ShowEditDelete = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.UsersAdmin);
            if (ShowEditDelete)
            {
                ViewData["ShowEditDelete"] = true;
            }


            // load user list
            var users = await _context.Users.ToListAsync();
            var husers = await _context.HelpdeskUsers.Include(x => x.Group).ToListAsync();
            UserList = new List<InputModel>();

            foreach (var u in users)
            {
                var h = husers.Where(x => x.IdentityUserId == u.Id).FirstOrDefault();
                if (h == null)
                {
                    h = new HelpdeskUser()
                    {
                        GivenName = "",
                        Surname = "",
                        JobTitle = "",
                        Company = "",
                        IsEnabled = true
                    };
                }
                UserList.Add(new InputModel()
                {
                    Id = u.Id,
                    Email = u.Email,
                    GivenName = h.GivenName,
                    Surname = h.Surname,
                    DisplayName = h.DisplayName,
                    JobTitle = h.JobTitle ?? "",
                    Company = h.Company ?? "",
                    Enabled = h.IsEnabled,
                    Group = h.Group?.Name ?? ""
                });
            }
            ViewData["SearchTerm"] = "Search...";
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string search)
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
            bool ShowEditDelete = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.UsersAdmin);
            if (ShowEditDelete)
            {
                ViewData["ShowEditDelete"] = true;
            }

            if (string.IsNullOrEmpty(search))
            {
                return await OnGetAsync();
            }

            // load user list
            var users = await _context.Users.ToListAsync();
            var husers = await _context.HelpdeskUsers.Include(x => x.Group)
                .ToListAsync();
            var tempList = new List<InputModel>();
            // build our searchable list.
            foreach (var u in users)
            {
                var h = husers.Where(x => x.IdentityUserId == u.Id).FirstOrDefault();
                if (h == null)
                {
                    h = new HelpdeskUser()
                    {
                        GivenName = "",
                        Surname = "",
                        JobTitle = "",
                        Company = "",
                        IsEnabled = true
                    };
                }
                tempList.Add(new InputModel()
                {
                    Id = u.Id,
                    Email = u.Email,
                    GivenName = h.GivenName,
                    Surname = h.Surname,
                    DisplayName = h.DisplayName,
                    JobTitle = h.JobTitle ?? "",
                    Company = h.Company ?? "",
                    Enabled = h.IsEnabled,
                    Group = h.Group?.Name ?? ""
                });
            }
            string sch = search.Trim().ToLower();
            // find the keyword search term in any of these fields.
            UserList = tempList.Where(x =>
                x.DisplayName.ToLower().Contains(sch) ||
                x.GivenName.ToLower().Contains(sch) ||
                x.Surname.ToLower().Contains(sch) ||
                x.Company.ToLower().Contains(sch) ||
                x.Group.ToLower().Contains(sch) ||
                x.JobTitle.ToLower().Contains(sch) ||
                x.Company.ToLower().Contains(sch) ||
                x.Email.ToLower().Contains(sch)).ToList();
            
            ViewData["SearchTerm"] = search;

            return Page();
        }
    }
}
