using Helpdesk.Authorization;
using Helpdesk.Data;
using Helpdesk.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;

namespace Helpdesk.Pages.ImportExport.People
{
    public class ImportFieldsModel : DI_BasePageModel
    {
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IEmailSender _emailSender;

        public ImportFieldsModel(ApplicationDbContext dbContext,
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
            [Required]
            public string TempFile { get; set; } = string.Empty!;
            [Display(Name ="Email Field")]
            [Required]
            public string EmailField { get; set; } = string.Empty!;
            [Display(Name ="Display Name Field")]
            [Required]
            public string DisplayNameField { get; set; } = string.Empty!;
            [Display(Name ="Given Name Field")]
            [Required]
            public string GivenNameField { get; set; } = string.Empty!;
            [Display(Name ="Surname Field")]
            [Required]
            public string SurnameField { get; set; } = string.Empty!;
            [Display(Name ="Job Title Field")]
            public string? JobTitleField { get; set; } = string.Empty!;
            [Display(Name ="Company Field")]
            public string? CompanyField { get; set; } = string.Empty!;
            [Display(Name = "Phone Number Field")]
            public string? PhoneNumberField { get; set; } = string.Empty!;
            [Display(Name = "Site Navigation Template Field")]
            public string? SiteNavTemplateField { get; set; } = string.Empty!;
            [Display(Name ="Group Name Field")]
            public string? GroupField { get; set; } = string.Empty!;
            [Display(Name ="Assigned Licenses Field")]
            public string? LicenseField { get; set; } = string.Empty!;
            [Required]
            [Display(Name = "Email users of new accounts")]
            public string NotifyUsers { get; set; } = string.Empty!;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public List<string> FieldNames { get; set; }

        public List<string> NotifyOptions = new List<string> { "Yes", "No" };

        public async Task<IActionResult> OnGetAsync(string? fileId)
        {
            await LoadSiteSettings(ViewData);
            if (_currentHelpdeskUser == null)
            {
                // This happens when a user logs in, but hasn't set up their profile yet.
                return Forbid();
                // For some pages, it might make sense to redirect to the account profile page so they can immediately enter their details.
                //return RedirectToPage("/Identity/Account/Manage");
            }
            bool HasClaim = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.ImportExport);
            if (!HasClaim)
            {
                return Forbid();
            }

            if (string.IsNullOrEmpty(fileId))
            { 
                return NotFound();
            }

            var fileupload = await _context.FileUploads.Where(x => x.Id == fileId).FirstOrDefaultAsync();
            if (fileupload == null)
            {
                return NotFound();
            }

            string? header = string.Empty;
            try
            {
                // TODO: This only works for filesystem files. It needs to be updated to work with 
                // database stored files.
                string filePath = await FileHelpers.GetActualFilePath(_context, fileupload);
                if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
                using (StreamReader reader = System.IO.File.OpenText(filePath))
                {
                    header = await reader.ReadLineAsync();
                }
            }
            catch 
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            if (string.IsNullOrEmpty(header))
            {
                return NotFound();
            }

            Input = new InputModel();
            Input.NotifyUsers = "Yes";
            Input.TempFile = fileId ?? "";

            string[] col = GrabColumbsAndProcess(header);

            return Page();
        }

        private string[] GrabColumbsAndProcess(string header)
        {
            string[] cols = CSVHelper.SplitCSVLine(header);
            MatchUpColumns(cols);

            FieldNames = new List<string>(cols);
            FieldNames.Insert(0, "");
            return cols;
        }

        private void MatchUpColumns(string[] cols)
        {
            string[] lcols = new string[cols.Length];
            for (int i = 0; i < cols.Length; i++)
            {
                lcols[i] = cols[i].ToLower();
            }

            // Match up Email Address
            int fieldid = Array.IndexOf(lcols, "user principal name");
            if (fieldid != -1)
            {
                Input.EmailField = cols[fieldid];
            }
            fieldid = Array.IndexOf(lcols, "email");
            if (fieldid != -1)
            {
                Input.EmailField = cols[fieldid];
            }
            fieldid = Array.IndexOf(lcols, "email address");
            if (fieldid != -1)
            {
                Input.EmailField = cols[fieldid];
            }
            fieldid = Array.IndexOf(lcols, "emailaddress");
            if (fieldid != -1)
            {
                Input.EmailField = cols[fieldid];
            }

            // Display Name
            fieldid = Array.IndexOf(lcols, "name");
            if (fieldid != -1)
            {
                Input.DisplayNameField = cols[fieldid];
            }
            fieldid = Array.IndexOf(lcols, "fullname");
            if (fieldid != -1)
            {
                Input.DisplayNameField = cols[fieldid];
            }
            fieldid = Array.IndexOf(lcols, "full name");
            if (fieldid != -1)
            {
                Input.DisplayNameField = cols[fieldid];
            }
            fieldid = Array.IndexOf(lcols, "display name");
            if (fieldid != -1)
            {
                Input.DisplayNameField = cols[fieldid];
            }
            fieldid = Array.IndexOf(lcols, "displayname");
            if (fieldid != -1)
            {
                Input.DisplayNameField = cols[fieldid];
            }

            // First Name
            fieldid = Array.IndexOf(lcols, "first name");
            if (fieldid != -1)
            {
                Input.GivenNameField = cols[fieldid];
            }
            fieldid = Array.IndexOf(lcols, "given name");
            if (fieldid != -1)
            {
                Input.GivenNameField = cols[fieldid];
            }
            fieldid = Array.IndexOf(lcols, "firstname");
            if (fieldid != -1)
            {
                Input.GivenNameField = cols[fieldid];
            }
            fieldid = Array.IndexOf(lcols, "givenname");
            if (fieldid != -1)
            {
                Input.GivenNameField = cols[fieldid];
            }

            // Last Name
            fieldid = Array.IndexOf(lcols, "surname");
            if (fieldid != -1)
            {
                Input.SurnameField = cols[fieldid];
            }
            fieldid = Array.IndexOf(lcols, "last name");
            if (fieldid != -1)
            {
                Input.SurnameField = cols[fieldid];
            }
            fieldid = Array.IndexOf(lcols, "lastname");
            if (fieldid != -1)
            {
                Input.SurnameField = cols[fieldid];
            }

            // Job Title
            fieldid = Array.IndexOf(lcols, "title");
            if (fieldid != -1)
            {
                Input.JobTitleField = cols[fieldid];
            }
            fieldid = Array.IndexOf(lcols, "job title");
            if (fieldid != -1)
            {
                Input.JobTitleField = cols[fieldid];
            }
            fieldid = Array.IndexOf(lcols, "jobtitle");
            if (fieldid != -1)
            {
                Input.JobTitleField = cols[fieldid];
            }

            // Company
            fieldid = Array.IndexOf(lcols, "company");
            if (fieldid != -1)
            {
                Input.CompanyField = cols[fieldid];
            }
            fieldid = Array.IndexOf(lcols, "company name");
            if (fieldid != -1)
            {
                Input.CompanyField = cols[fieldid];
            }
            fieldid = Array.IndexOf(lcols, "companyname");
            if (fieldid != -1)
            {
                Input.CompanyField = cols[fieldid];
            }

            // Phone Number
            fieldid = Array.IndexOf(lcols, "phone");
            if (fieldid != -1)
            {
                Input.PhoneNumberField = cols[fieldid];
            }
            fieldid = Array.IndexOf(lcols, "phone number");
            if (fieldid != -1)
            {
                Input.PhoneNumberField = cols[fieldid];
            }
            fieldid = Array.IndexOf(lcols, "phonenumber");
            if (fieldid != -1)
            {
                Input.PhoneNumberField = cols[fieldid];
            }

            // Site nav template
            fieldid = Array.IndexOf(lcols, "sitenavtemplate");
            if (fieldid != -1)
            {
                Input.SiteNavTemplateField = cols[fieldid];
            }
            fieldid = Array.IndexOf(lcols, "site nav template");
            if (fieldid != -1)
            {
                Input.SiteNavTemplateField = cols[fieldid];
            }
            fieldid = Array.IndexOf(lcols, "site navigation template");
            if (fieldid != -1)
            {
                Input.SiteNavTemplateField = cols[fieldid];
            }
            fieldid = Array.IndexOf(lcols, "site navigation");
            if (fieldid != -1)
            {
                Input.SiteNavTemplateField = cols[fieldid];
            }
            fieldid = Array.IndexOf(lcols, "site nav");
            if (fieldid != -1)
            {
                Input.SiteNavTemplateField = cols[fieldid];
            }
            
            // Group
            fieldid = Array.IndexOf(lcols, "group");
            if (fieldid != -1)
            {
                Input.GroupField = cols[fieldid];
            }
            fieldid = Array.IndexOf(lcols, "group name");
            if (fieldid != -1)
            {
                Input.GroupField = cols[fieldid];
            }
            fieldid = Array.IndexOf(lcols, "groupname");
            if (fieldid != -1)
            {
                Input.GroupField = cols[fieldid];
            }

            // License
            fieldid = Array.IndexOf(lcols, "license");
            if (fieldid != -1)
            {
                Input.LicenseField = cols[fieldid];
            }
            fieldid = Array.IndexOf(lcols, "licenses");
            if (fieldid != -1)
            {
                Input.LicenseField = cols[fieldid];
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
            bool HasClaim = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.ImportExport);
            if (!HasClaim)
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var fileupload = await _context.FileUploads.Where(x => x.Id == Input.TempFile).FirstOrDefaultAsync();
            if (fileupload == null)
            {
                return NotFound();
            }
            // Go ahead and look up the defautl site nav template in case we need it.
            var defaultTemplateOpt = _context.ConfigOpts
                .Where(x => x.Category == ConfigOptConsts.Accounts_DefaultNavTemplate.Category &&
                            x.Key == ConfigOptConsts.Accounts_DefaultNavTemplate.Key)
                .FirstOrDefault();
            string defaultSiteNavTemplate = defaultTemplateOpt?.Value ?? ConfigOptConsts.Accounts_DefaultNavTemplate.Value;
            var siteNavTemplateLoadDefault = await _context.SiteNavTemplates
                .Where(x => x.Name == defaultSiteNavTemplate)
                .FirstOrDefaultAsync();
            // create a dictionary to quicly find these as we import.
            Dictionary<string, SiteNavTemplate> TemplateCache = new Dictionary<string, SiteNavTemplate>();
            if (siteNavTemplateLoadDefault != null)
            {
                // if we have one, add default site nav template.
                TemplateCache.Add(defaultSiteNavTemplate, siteNavTemplateLoadDefault);
            }

            // Go ahead and load in the groups.
            var groups = await _context.Groups.ToListAsync();
            Dictionary<string, Group> GroupCache = new Dictionary<string, Group>();
            foreach (var g in groups)
            {
                GroupCache.Add(g.Name, g);
            }

            // Go ahead and load in the license types. We only handle user licenses that don't require a product code entry.
            var licensetypes = await _context.LicenseType
                .Where(x => x.IsUserLicense == true && x.UserRequireProductCode == false)
                .ToListAsync();
            Dictionary<string, LicenseType> LicenseTypeCache = new Dictionary<string, LicenseType>();
            foreach (var l in licensetypes)
            {
                LicenseTypeCache.Add(l.Name, l);
            }

            // Get the upload directory where we will be saving the log file to.
            ConfigOpt? opt = await _context.ConfigOpts
                .Where(x => x.Category == ConfigOptConsts.System_UploadPath.Category &&
                            x.Key == ConfigOptConsts.System_UploadPath.Key)
                .FirstOrDefaultAsync();
            string targetFilePath = opt?.Value ?? ConfigOptConsts.System_UploadPath.Value;
            // generate the random file name.
            var trustedFileNameForFileStorage = Path.GetRandomFileName();
            var filePath = Path.Combine(
                targetFilePath, trustedFileNameForFileStorage);

            var doctype = await _context.DocumentTypes
                .Where(x => x.IsSystemType && x.Name == DocumentTypeStrings.ImportUsersResults)
                .FirstOrDefaultAsync();

            // create the log file database entry
            FileUpload ImportLogDB = new FileUpload()
            {
                FilePath = trustedFileNameForFileStorage,
                IsTempFile = true,
                FileLength = 0,
                OriginalFileName = WebUtility.HtmlEncode(String.Format("User Import Log {0}.txt", DateTime.UtcNow.ToString())),
                IsDatabaseFile = false,
                UploadedBy = _currentHelpdeskUser.IdentityUserId,
                MIMEType = "text/plain",
                FileData = null,
                WhenUploaded = DateTime.UtcNow,
                DocumentType = doctype,
                AllowAllAuthenticatedAccess = false,
                AllowUnauthenticatedAccess = false
            };

            _context.FileUploads.Add(ImportLogDB);
            await _context.SaveChangesAsync();
            string? header = string.Empty;
            try
            {
                // TODO: This only works for filesystem files. It needs to be updated to work with 
                // database stored files.
                string readfilePath = await FileHelpers.GetActualFilePath(_context, fileupload);
                if (string.IsNullOrEmpty(readfilePath) || !System.IO.File.Exists(readfilePath))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
                using (StreamReader reader = System.IO.File.OpenText(readfilePath))
                using (StreamWriter writer = System.IO.File.CreateText(filePath))
                {
                    header = await reader.ReadLineAsync();

                    if (string.IsNullOrEmpty(header))
                    {
                        return NotFound();
                    }

                    string[] cols = CSVHelper.SplitCSVLine(header);

                    int EmailField = Array.IndexOf(cols, Input.EmailField);
                    int DisplayNameField = Array.IndexOf(cols, Input.DisplayNameField);
                    int GivenNameField = Array.IndexOf(cols, Input.GivenNameField);
                    int SurnameField = Array.IndexOf(cols, Input.SurnameField);
                    int JobTitleField = Array.IndexOf(cols, Input.JobTitleField);
                    int CompanyField = Array.IndexOf(cols, Input.CompanyField);
                    int PhoneNumberField = Array.IndexOf(cols, Input.PhoneNumberField);
                    int SiteNavTemplateField = Array.IndexOf(cols, Input.SiteNavTemplateField);
                    int GroupField = Array.IndexOf(cols, Input.GroupField);
                    int LicenseField = Array.IndexOf(cols, Input.LicenseField);

                    bool Retry = false;
                    if (EmailField == -1)
                    {
                        ModelState.AddModelError("Input.EmailField", "This field is required to import.");
                        Retry = true;
                    }
                    if (DisplayNameField == -1)
                    {
                        ModelState.AddModelError("Input.DisplayNameField", "This field is required to import.");
                        Retry = true;
                    }
                    if (GivenNameField == -1)
                    {
                        ModelState.AddModelError("Input.GivenNameField", "This field is required to import.");
                        Retry = true;
                    }
                    if (SurnameField == -1)
                    {
                        ModelState.AddModelError("Input.SurnameField", "This field is required to import.");
                        Retry = true;
                    }

                    if (Retry)
                    {
                        string[] col = GrabColumbsAndProcess(header);
                        return Page();
                    }

                    // loop over all the rows until we run out and create an account for each user as we go.
                    // input must be validated to make values are valid.
                    string? line = string.Empty;

                    // We're assuming line 1 as the header, so first record line starts with 2
                    int LineCount = 1;
                    int CompletedCount = 0;
                    int FailParseCount = 0;
                    int FailImportCount = 0;
                    StringBuilder log = new StringBuilder();

                    while (!string.IsNullOrEmpty(line = await reader.ReadLineAsync()))
                    {
                        LineCount++;
                        try
                        {
                            // Split the line
                            var fields = CSVHelper.SplitCSVLine(line);
                            if (fields.Length != cols.Length)
                            {
                                // make sure we have the expected number of columns
                                FailParseCount++;
                                log.AppendFormat("Failed to parse line {0}. Column count doesn't match header.\r\n", LineCount);
                                continue;
                            }
                            string csvEmail = fields[EmailField];
                            var existUser = await _userManager.FindByEmailAsync(csvEmail);
                            if (existUser != null)
                            {
                                // Make sure the email address isn't already taken.
                                FailImportCount++;
                                log.AppendFormat("Failed to import line {0}. Email {1} is already taken.\r\n", LineCount, csvEmail);
                                continue;
                            }

                            string csvSiteNavTemplate = defaultSiteNavTemplate;
                            if (SiteNavTemplateField != -1)
                            {
                                // Look up site nav template, or use the default.
                                SiteNavTemplate? tmp;
                                if (TemplateCache.TryGetValue(fields[SiteNavTemplateField], out tmp))
                                {
                                    csvSiteNavTemplate = tmp.Name;
                                }
                                else
                                {
                                    tmp = await _context.SiteNavTemplates
                                        .Where(x => x.Name == fields[SiteNavTemplateField])
                                        .FirstOrDefaultAsync();
                                    if (tmp != null)
                                    {
                                        TemplateCache.Add(tmp.Name, tmp);
                                        csvSiteNavTemplate = tmp.Name;
                                    }
                                    else
                                    {
                                        log.AppendFormat("Warning on line {0} for Email {1}: Site Nav Template {2} doesn't exist.\r\n", LineCount, csvEmail, fields[SiteNavTemplateField]);
                                    }
                                }
                            }

                            string? csvGroup = null;
                            if (GroupField != -1)
                            {
                                Group? tmp;
                                if (GroupCache.TryGetValue(fields[GroupField], out tmp))
                                {
                                    csvGroup = tmp.Name;
                                }
                                else
                                {
                                    log.AppendFormat("Warning on line {0} for Email {1}: Group {2} doesn't exist.\r\n", LineCount, csvEmail, fields[GroupField]);
                                    csvGroup = null;
                                }
                            }

                            string csvDisplayName = fields[DisplayNameField];
                            string csvGivenName = fields[GivenNameField];
                            string csvSurname = fields[SurnameField];
                            string? csvJobTitle = JobTitleField == -1 ? null : fields[JobTitleField];
                            string? csvCompany = CompanyField == -1 ? null : fields[CompanyField];
                            string? csvPhoneNumber = PhoneNumberField == -1 ? null : fields[PhoneNumberField];
                            string? csvLicenses = LicenseField == -1 ? null : fields[LicenseField];

                            // got all our required fields.  Create the user now.
                            var iUser = Activator.CreateInstance<IdentityUser>();
                            await _userStore.SetUserNameAsync(iUser, csvEmail, CancellationToken.None);
                            await _emailStore.SetEmailAsync(iUser, csvEmail, CancellationToken.None);
                            var result = await _userManager.CreateAsync(iUser);
                            if (!result.Succeeded)
                            {
                                FailImportCount++;
                                log.AppendFormat("Failed to import line {0}. Failure creating account for Email {1}\r\n", LineCount, csvEmail);
                                continue;
                            }
                            // set phone number
                            if (!string.IsNullOrEmpty(csvPhoneNumber))
                            {
                                await _userManager.SetPhoneNumberAsync(iUser, csvPhoneNumber);
                            }

                            // automatically confirm the email now.
                            string token = await _userManager.GenerateEmailConfirmationTokenAsync(iUser);
                            await _userManager.ConfirmEmailAsync(iUser, token);

                            SiteNavTemplate assignedTemplate = TemplateCache[csvSiteNavTemplate];

                            Group? assignedGroup = null;
                            if (!string.IsNullOrEmpty(csvGroup))
                            {
                                assignedGroup = GroupCache[csvGroup];
                            }

                            // create the HelpdeskUser
                            bool enabled = true;
                            var hUser = new HelpdeskUser()
                            {
                                IdentityUserId = iUser.Id,
                                IsEnabled = enabled,
                                GivenName = csvGivenName,
                                Surname = csvSurname,
                                DisplayName = csvDisplayName,
                                JobTitle = csvJobTitle,
                                Company = csvCompany,
                                SiteNavTemplate = assignedTemplate,
                                Group = assignedGroup
                            };
                            _context.HelpdeskUsers.Add(hUser);
                            await _context.SaveChangesAsync();

                            if (Input.NotifyUsers == "Yes")
                            {
                                // if Notify is Yes, send new account notification
                                await SendNewUserEmail(iUser, hUser, csvEmail);
                            }

                            if (!string.IsNullOrEmpty(csvLicenses))
                            {
                                foreach (var lic in LicenseTypeCache)
                                {
                                    if (csvLicenses.Contains(lic.Value.Name, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        var ula = new UserLicenseAssignment()
                                        {
                                            HelpdeskUser = hUser,
                                            LicenseType = lic.Value,
                                            ProductCode = string.Empty
                                        };
                                        _context.UserLicenseAssignments.Add(ula);
                                        await _context.SaveChangesAsync();
                                        _context.Entry(ula).State = EntityState.Detached;
                                    }
                                } 
                            }

                            if (Input.NotifyUsers == "Yes")
                            {
                                // if Notify is Yes, send new account notification
                                await SendNewUserEmail(iUser, hUser, csvEmail);
                            }

                            _context.Entry(hUser).State = EntityState.Detached;
                            _context.Entry(iUser).State = EntityState.Detached;
                            hUser = null;
                            iUser = null;

                            CompletedCount++;

                        }
                        catch (Exception ex)
                        {
                            FailImportCount++;
                            log.AppendFormat("Exception processing line {0}: {1}\r\n", LineCount, ex.Message);
                        }
                    }
                    //Write out results summary, and then write out the full log.
                    writer.WriteLine("Result of User Import");
                    writer.WriteLine("CSV Lines Read: {0}", LineCount);
                    writer.WriteLine("Parse errors: {0}", FailParseCount);
                    writer.WriteLine("Imports Failed: {0}", FailImportCount);
                    writer.WriteLine("Imports Successful: {0}", CompletedCount);
                    writer.WriteLine("Error Log:");
                    writer.Write(log.ToString());
                    log.Clear();
                }
                FileInfo logInfo = new FileInfo(filePath);
                ImportLogDB.FileLength = (int)Math.Min(Int32.MaxValue, logInfo.Length);
                _context.FileUploads.Update(ImportLogDB);
                await _context.SaveChangesAsync();
                return RedirectToPage("./ImportResult", new { fileId = ImportLogDB.Id });
            }
            catch
            {
                ModelState.AddModelError("", "Failed to process the uploaded file");
            }
            return Page();
        }

        private async Task SendNewUserEmail(IdentityUser iUser, HelpdeskUser hUser, string email)
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
                email,
                $"New {siteName} Account Created",
                body.ToString());
        }
    }
}
