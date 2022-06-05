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

namespace Helpdesk.Pages.LicenseTypes
{
    public class DeleteModel : DI_BasePageModel
    {
        public DeleteModel(ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
            : base(dbContext, userManager, signInManager)
        { }
        public class InputModel
        {
            [Required]
            public int Id { get; set; }
            [Required]
            public string Name { get; set; }
            [Required]
            public string Description { get; set; }
            public int? Seats { get; set; }
            [Required]
            [Display(Name = "Device License")]
            public bool IsDeviceLicense { get; set; }
            [Required]
            [Display(Name = "User License")]
            public bool IsUserLicense { get; set; }
            [Required]
            [Display(Name = "Device Requires Product Code")]
            public bool DeviceRequireProductCode { get; set; }
            [Required]
            [Display(Name = "User Requires Product Code")]
            public bool UserRequireProductCode { get; set; }
            [Required]
            public string Status { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [Display(Name = "Assigned Users")]
        public int UserCount { get; set; }

        [Display(Name = "Assigned Devices")]
        public int DeviceCount { get; set; }

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
            bool HasClaim = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.LicenseTypeAdmin);
            if (!HasClaim)
            {
                return Forbid();
            }
            if (id == null || _context.LicenseType == null)
            {
                return NotFound();
            }

            var licensetype = await _context.LicenseType.FirstOrDefaultAsync(m => m.Id == id);

            if (licensetype == null)
            {
                return NotFound();
            }
            else 
            {
                Input = new InputModel()
                {
                    Id = licensetype.Id,
                    Name = licensetype.Name,
                    Description = licensetype.Description,
                    Seats = licensetype.Seats,
                    IsDeviceLicense = licensetype.IsDeviceLicense,
                    IsUserLicense = licensetype.IsUserLicense,
                    DeviceRequireProductCode = licensetype.DeviceRequireProductCode,
                    UserRequireProductCode = licensetype.UserRequireProductCode,
                    Status = licensetype.Status.ToString()
                };
                UserCount = await _context.UserLicenseAssignments.Where(x => x.LicenseType.Id == id).CountAsync();
                DeviceCount = await _context.AssetLicenseAssignments.Where(x => x.LicenseType.Id == id).CountAsync();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.LicenseType == null)
            {
                return NotFound();
            }
            var licensetype = await _context.LicenseType.FindAsync(id);

            if (licensetype != null)
            {
                var ul = await _context.UserLicenseAssignments.Where(x => x.LicenseType.Id == id).ToListAsync();
                _context.UserLicenseAssignments.RemoveRange(ul);
                var al = await _context.AssetLicenseAssignments.Where(x => x.LicenseType.Id == id).ToListAsync();
                _context.AssetLicenseAssignments.RemoveRange(al);
                await _context.SaveChangesAsync();

                _context.LicenseType.Remove(licensetype);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
