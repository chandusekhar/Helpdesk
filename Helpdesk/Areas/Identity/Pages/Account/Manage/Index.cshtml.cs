// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Helpdesk.Authorization;
using Helpdesk.Data;
using Helpdesk.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : DI_BasePageModel
    {

        public IndexModel(
            ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
            :base(dbContext, userManager, signInManager)
        {
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Required]
            [Display(Name = "Given Name")]
            public string GivenName { get; set; }
            [Required]
            public string Surname { get; set; }
            [Display(Name = "Display Name")]
            public string DisplayName { get; set; }
            [Display(Name = "Job Title")]
            public string JobTitle { get; set; }
            public string Company { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var huser = await _context.HelpdeskUsers.Where(x => x.IdentityUserId == user.Id).FirstOrDefaultAsync();

            Username = userName;
            string displayName = huser?.DisplayName ?? "";
            if (string.IsNullOrEmpty(displayName))
            {
                displayName = huser?.GivenName ?? "";
            }

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                GivenName = huser?.GivenName ?? "",
                Surname = huser?.Surname ?? "",
                DisplayName = displayName,
                JobTitle = huser?.JobTitle ?? "",
                Company = huser?.Company ?? ""
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!await LoadSiteSettings(ViewData))
            {
                return RedirectToPage("/Index");
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!await LoadSiteSettings(ViewData))
            {
                return RedirectToPage("/Index");
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            if (string.IsNullOrEmpty(Input.DisplayName))
            {
                Input.DisplayName = Input.GivenName;
            }

            var huser = await _context.HelpdeskUsers.Where(x => x.IdentityUserId == user.Id).FirstOrDefaultAsync();
            if (huser == null)
            {
                var templateName = _context.ConfigOpts
                    .Where(x => x.Category == ConfigOptConsts.Accounts_DefaultNavTemplate.Category &&
                                x.Key == ConfigOptConsts.Accounts_DefaultNavTemplate.Key)
                    .FirstOrDefault();
                if (templateName == null)
                {
                    templateName = new ConfigOpt()
                    {
                        Category = ConfigOptConsts.Accounts_DefaultNavTemplate.Category,
                        Key = ConfigOptConsts.Accounts_DefaultNavTemplate.Key,
                        Value = ConfigOptConsts.Accounts_DefaultNavTemplate.Value
                    };
                }
                var defaultTemplate = await _context.SiteNavTemplates
                    .Where(x => x.Name == templateName.Value)
                    .FirstOrDefaultAsync();
                huser = new HelpdeskUser()
                {
                    IdentityUserId = user.Id,
                    IsEnabled = true,
                    GivenName = Input.GivenName,
                    Surname = Input.Surname,
                    DisplayName = Input.DisplayName,
                    JobTitle = Input.JobTitle,
                    Company = Input.Company,
                    SiteNavTemplate = defaultTemplate
                };
                _context.HelpdeskUsers.Add(huser);
                // if this is the first user, then give it super admin permissions automatically.
                bool ExistingUsers = await _context.HelpdeskUsers.AnyAsync();
                if (!ExistingUsers)
                {
                    await _context.SaveChangesAsync();
                    await RightsManagement.UserAddRole(_context, user.Id, RoleConstantStrings.SuperAdmin);
                }
                
            }
            else
            {
                huser.GivenName = Input.GivenName;
                huser.Surname = Input.Surname;
                huser.JobTitle = Input.JobTitle;
                huser.Company = Input.Company;
                huser.DisplayName = Input.DisplayName;
                _context.HelpdeskUsers.Update(huser);
            }
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }
            await _context.SaveChangesAsync();
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
