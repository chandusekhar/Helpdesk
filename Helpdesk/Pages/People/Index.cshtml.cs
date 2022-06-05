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
            public string Email { get; set; }
            [Required]
            public string GivenName { get; set; }
            [Required]
            public string Surname { get; set; }
            public string? JobTitle { get; set; }
            public string? Company { get; set; }
            [Required]
            public bool Enabled { get; set; }
            public string? Group { get; set; }
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
                    JobTitle = h.JobTitle,
                    Company = h.Company,
                    Enabled = h.IsEnabled,
                    Group = h.Group?.Name
                });
            }

            return Page();
        }
    }
}
