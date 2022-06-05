using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Helpdesk.Data;
using Helpdesk.Infrastructure;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Helpdesk.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk.Pages.LicenseTypes
{
    public class CreateModel : DI_BasePageModel
    {

        public CreateModel(ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
            : base(dbContext, userManager, signInManager)
        { }

        public class InputModel
        {
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
            Input = new InputModel();
            return Page();
        }


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
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

            var lic = new LicenseType()
            {
                Name = Input.Name,
                Description = Input.Description,
                Seats = Input.Seats,
                IsDeviceLicense = Input.IsDeviceLicense,
                IsUserLicense = Input.IsUserLicense,
                DeviceRequireProductCode = Input.DeviceRequireProductCode,
                UserRequireProductCode = Input.UserRequireProductCode,
                Status = status
            };

            _context.LicenseType.Add(lic);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
