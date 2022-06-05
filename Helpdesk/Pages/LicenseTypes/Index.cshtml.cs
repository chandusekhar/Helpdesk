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

namespace Helpdesk.Pages.LicenseTypes
{
    public class IndexModel : DI_BasePageModel
    {

        public IndexModel(ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
            : base(dbContext, userManager, signInManager)
        { }

        public class InputView
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty!;
            public string Status { get; set; } = string.Empty!;
            public string Seats { get; set; } = string.Empty!;
        }

        public IList<InputView> Input { get; set; } = new List<InputView>();

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
            bool HasClaim = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.LicenseTypeAdmin);
            if (!HasClaim)
            {
                return Forbid();
            }

            if (_context.LicenseType != null)
            {
                var licenses = await _context.LicenseType.ToListAsync();
                foreach (var lic in licenses)
                {
                    int ucount = await _context.UserLicenseAssignments.Where(x => x.LicenseType.Id == lic.Id).CountAsync();
                    int acount = await _context.AssetLicenseAssignments.Where(x => x.LicenseType.Id == lic.Id).CountAsync();
                    string seat = (ucount + acount).ToString();
                    if (lic.Seats.HasValue)
                    {
                        seat = seat + " / " + lic.Seats.Value.ToString();
                    }
                    Input.Add(new InputView()
                    {
                        Id = lic.Id,
                        Name = lic.Name,
                        Status = lic.Status.ToString(),
                        Seats = seat
                    });
                }
            }
            return Page();
        }
    }
}
