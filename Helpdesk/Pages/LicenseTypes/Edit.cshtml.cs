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
using System.ComponentModel.DataAnnotations;
using Helpdesk.Authorization;

namespace Helpdesk.Pages.LicenseTypes
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

        public List<string> LicenseStatusOptions = new List<string>()
        {
            LicenseStatuses.Hidden.ToString(),
            LicenseStatuses.Active.ToString(),
            LicenseStatuses.Expired.ToString(),
            LicenseStatuses.Retired.ToString()
        };
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

            var licensetype =  await _context.LicenseType.FirstOrDefaultAsync(m => m.Id == id);
            if (licensetype == null)
            {
                return NotFound();
            }

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
            bool HasClaim = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.LicenseTypeAdmin);
            if (!HasClaim)
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            LicenseStatuses status = LicenseStatuses.Hidden;
            switch (Input.Status)
            {
                case "Hidden":
                    status = LicenseStatuses.Hidden;
                    break;
                case "Active":
                    status = LicenseStatuses.Active;
                    break;
                case "Expired":
                    status = LicenseStatuses.Expired;
                    break;
                case "Retired":
                    status = LicenseStatuses.Retired;
                    break;
                default:
                    throw new Exception(string.Format("License Status {0} not recognized.", Input.Status));
            }

            var lic = await _context.LicenseType
                .Where(x => x.Id == Input.Id)
                .FirstOrDefaultAsync();
            if (lic == null)
            {
                return NotFound();
            }

            lic.Name = Input.Name;
            lic.Description = Input.Description;
            lic.Seats = Input.Seats;
            lic.IsDeviceLicense = Input.IsDeviceLicense;
            lic.IsUserLicense = Input.IsUserLicense;
            lic.DeviceRequireProductCode = Input.DeviceRequireProductCode;
            lic.UserRequireProductCode = Input.UserRequireProductCode;
            lic.Status = status;
            
            _context.LicenseType.Update(lic);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

    }
}
