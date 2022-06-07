using Helpdesk.Authorization;
using Helpdesk.Data;
using Helpdesk.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Helpdesk.Pages.People
{
    public class ImportModel : DI_BasePageModel
    {

        public ImportModel(ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
            : base(dbContext, userManager, signInManager)
        { }

        [BindProperty]
        public BufferedSingleFileUploadPhysical FileUpload { get; set; }

        public string Result { get; private set; }

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

            return Page();
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
            bool HasClaim = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.UsersAdmin);
            if (!HasClaim)
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            ConfigOpt? opt = await _context.ConfigOpts
                .Where(x => x.Category == ConfigOptConsts.System_UploadPath.Category &&
                            x.Key == ConfigOptConsts.System_UploadPath.Key)
                .FirstOrDefaultAsync();
            string targetFilePath = opt?.Value ?? ConfigOptConsts.System_UploadPath.Value;

            opt = await _context.ConfigOpts
                .Where(x => x.Category == ConfigOptConsts.System_UploadFileSizeLimit.Category &&
                            x.Key == ConfigOptConsts.System_UploadFileSizeLimit.Key)
                .FirstOrDefaultAsync();
            int fileSizeLimit = Int32.Parse(opt?.Value ?? ConfigOptConsts.System_UploadFileSizeLimit.Value);

            string[] permittedExts = { ".csv" };

            var formFileContent =
                await FileHelpers.ProcessFormFile<BufferedSingleFileUploadPhysical>(
                    FileUpload.FormFile, ModelState, permittedExts,
                    fileSizeLimit);

            if (!ModelState.IsValid)
            {
                Result = "Please verify the upload file meets requirements.";
                return Page();
            }


            // For the file name of the uploaded file stored
            // server-side, use Path.GetRandomFileName to generate a safe
            // random file name.
            var trustedFileNameForFileStorage = Path.GetRandomFileName();
            var filePath = Path.Combine(
                targetFilePath, trustedFileNameForFileStorage);

            using (var fileStream = System.IO.File.Create(filePath))
            {
                await fileStream.WriteAsync(formFileContent);

                // To work directly with a FormFile, use the following
                // instead:
                //await FileUpload.FormFile.CopyToAsync(fileStream);
            }

            FileUpload upload = new FileUpload()
            {
                FilePath = filePath,
                IsTempFile = true,
                FileLength = formFileContent.Length,
                OriginalFileName = WebUtility.HtmlEncode(FileUpload.FormFile.FileName),
                IsDatabaseFile = false,
                UploadedBy = _currentHelpdeskUser.IdentityUserId,
                DetectedFileType = "text/csv",
                FileData = null,
                WhenUploaded = DateTime.UtcNow
            };

            _context.FileUploads.Add(upload);
            await _context.SaveChangesAsync();

            return RedirectToPage("ImportFields", new { fileId = upload.Id });
        }

        public class BufferedSingleFileUploadPhysical
        {
            [Required]
            [Display(Name = "File")]
            public IFormFile FormFile { get; set; }

            //[Display(Name = "Note")]
            //[StringLength(50, MinimumLength = 0)]
            //public string Note { get; set; }
        }

    }
}
