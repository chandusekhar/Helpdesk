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
            public string GivenName { get; set; }
            [Required]
            public string Surname { get; set; }
            public string JobTitle { get; set; }
            public string Company { get; set; }
        }

        private async Task LoadAsync(IdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var huser = await _context.HelpdeskUsers.Where(x => x.IdentityUserId == user.Id).FirstOrDefaultAsync();

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                GivenName = huser?.GivenName ?? "",
                Surname = huser?.Surname ?? "",
                JobTitle = huser?.JobTitle ?? "",
                Company = huser?.Company ?? ""
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadSiteSettings(ViewData);
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
            await LoadSiteSettings(ViewData);
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
                    GivenName = Input.GivenName,
                    Surname = Input.Surname,
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
