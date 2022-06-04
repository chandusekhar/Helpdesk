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
using Helpdesk.Authorization;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Encodings.Web;

namespace Helpdesk.Pages.People
{
    public class CreateModel : DI_BasePageModel
    {
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IEmailSender _emailSender;

        public CreateModel(ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IUserStore<IdentityUser> userStore,
            IEmailSender emailSender)
            : base(dbContext, userManager, signInManager)
        {
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _emailSender = emailSender;
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }

        public class InputModel
        {
            [EmailAddress]
            [Required]
            public string Email { get; set; } = string.Empty;
            [Required]
            [Display(Name = "Given Name")]
            public string GivenName { get; set; } = string.Empty;
            [Required]
            public string Surname { get; set; } = string.Empty;
            [Required]
            [Display(Name = "Dispaly Name")]
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
            [Required]
            [Display(Name = "Notify New User")]
            public string NotifyUser { get; set; }
        }

        public List<string> SiteNavTemplates { get; set; } = new List<string>();

        public List<string> EnabledOptions { get; set; } = new List<string> { "Enabled", "Disabled" };
        public List<string> NotifyOptions { get; set; } = new List<string> { "Yes", "No" };

        [BindProperty]
        public InputModel Input { get; set; } = default!;

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
            bool HasClaim = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.UsersAdmin);
            if (!HasClaim)
            {
                return Forbid();
            }
            SiteNavTemplates = await _context.SiteNavTemplates.Select(x => x.Name).ToListAsync();
            var defaultTemplateName = _context.ConfigOpts
                .Where(x => x.Category == ConfigOptConsts.Accounts_DefaultNavTemplate.Category &&
                            x.Key == ConfigOptConsts.Accounts_DefaultNavTemplate.Key)
                .FirstOrDefault();
            Input = new InputModel()
            {
                Enabled = "Enabled",
                SiteNavTemplateName = defaultTemplateName?.Value ?? string.Empty,
                NotifyUser = "Yes"
            };
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
            bool HasClaim = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.UsersAdmin);
            if (!HasClaim)
            {
                return Forbid();
            }
            if (!ModelState.IsValid || _context.HelpdeskUsers == null || Input == null)
            {
                return Page();
            }

            // first, see if this email address is taken
            if ((await _userManager.FindByEmailAsync(Input.Email)) != null)
            {
                ModelState.AddModelError("Input.Email", "That email address is already taken.");
                SiteNavTemplates = await _context.SiteNavTemplates.Select(x => x.Name).ToListAsync();
                return Page();
            }

            // verify template is valid
            var navTemplate = await _context.SiteNavTemplates
                .Where(x => x.Name == Input.SiteNavTemplateName)
                .FirstOrDefaultAsync();
            if (navTemplate == null)
            {
                ModelState.AddModelError("Input.SiteNavTemplateName", "Select a valid Sate Nav Template.");
                SiteNavTemplates = await _context.SiteNavTemplates.Select(x => x.Name).ToListAsync();
                return Page();
            }

            var iUser = Activator.CreateInstance<IdentityUser>();
            await _userStore.SetUserNameAsync(iUser, Input.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(iUser, Input.Email, CancellationToken.None);
            var result = await _userManager.CreateAsync(iUser);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                SiteNavTemplates = await _context.SiteNavTemplates.Select(x => x.Name).ToListAsync();
                return Page();
            }
            await _userManager.SetPhoneNumberAsync(iUser, Input.PhoneNumber);

            // automatically confirm the email now.
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(iUser);
            await _userManager.ConfirmEmailAsync(iUser, token);

            // create the HelpdeskUser
            bool enabled = Input.Enabled == "Enabled";
            var hUser = new HelpdeskUser()
            {
                IdentityUserId = iUser.Id,
                IsEnabled = enabled,
                GivenName = Input.GivenName,
                Surname = Input.Surname,
                DisplayName = Input.DisplayName,
                JobTitle = Input.JobTitle,
                Company = Input.Company,
                SiteNavTemplate = navTemplate
            };
            _context.HelpdeskUsers.Add(hUser);
            await _context.SaveChangesAsync();

            // if Notify is Yes, send new account notification
            await SendNewUserEmail(iUser, hUser);
            return RedirectToPage("./Edit", new { Id = iUser.Id });
        }

        private async Task SendNewUserEmail(IdentityUser iUser, HelpdeskUser hUser)
        {
            string siteName = (await _context.ConfigOpts
                .Where(x => x.Category == ConfigOptConsts.Branding_SiteName.Category &&
                            x.Key == ConfigOptConsts.Branding_SiteName.Key)
                .FirstOrDefaultAsync())?.Value ?? ConfigOptConsts.Branding_SiteName.Value;

            //string orgName = (await _context.ConfigOpts
            //    .Where(x => x.Category == ConfigOptConsts.Branding_OrganizationName.Category &&
            //                x.Key == ConfigOptConsts.Branding_OrganizationName.Key)
            //    .FirstOrDefaultAsync())?.Value ?? ConfigOptConsts.Branding_OrganizationName.Value;

            string url = (await _context.ConfigOpts
                .Where(x => x.Category == ConfigOptConsts.Branding_SiteURL.Category &&
                            x.Key == ConfigOptConsts.Branding_SiteURL.Key)
                .FirstOrDefaultAsync())?.Value ?? ConfigOptConsts.Branding_SiteURL.Value;

            var code = await _userManager.GeneratePasswordResetTokenAsync(iUser);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ResetPassword",
                pageHandler: null,
                values: new { area = "Identity", code },
                protocol: Request.Scheme);
            StringBuilder body = new StringBuilder();
            body.AppendFormat("<h3>Hello {0},</h3>", hUser.DisplayName);
            body.AppendFormat("<p>A new {0} account has been created for you by {1}.</p>", siteName, _currentHelpdeskUser.DisplayName);
            body.AppendFormat("<p>Please Bookmark the site's URL for future access: <a href='{0}'>{1}</a></p>", url, url);
            body.AppendFormat("<p>Before you can access the {0} site, you need to confirm your email address and create a new password.</p>", siteName);
            body.Append("<p>Please DO NOT reuse any password that you have used on any other site, and especially <b>DO NOT enter your email account password</b>.</p>");
            body.AppendFormat("<p>Confirm your account by <a href='{0}'>clicking here</a></p>", HtmlEncoder.Default.Encode(callbackUrl));

            await _emailSender.SendEmailAsync(
                Input.Email,
                $"New {siteName} Account Created",
                body.ToString());
        }
    }
}
