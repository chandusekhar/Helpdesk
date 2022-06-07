using Helpdesk.Data;
using Helpdesk.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk.Pages
{
    public class FileDownloadPageModel : DI_BasePageModel
    {

        public FileDownloadPageModel(ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
            : base(dbContext, userManager, signInManager)
        { }
        
        [BindProperty]
        public string fileid { get; set; }

        public void OnGet()
        {

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

            if (string.IsNullOrEmpty(fileid))
            {
                return RedirectToPage("./Index");
            }

            var file = await _context.FileUploads.Where(x => x.Id == fileid).FirstOrDefaultAsync();
            if (file == null)
            {
                return NotFound();
            }

            // perform permission validation.  For now, the uploaded can download their own files.
            if (file.UploadedBy == _currentHelpdeskUser.IdentityUserId)
            {
                if (!file.IsDatabaseFile)
                {
                    if (file.FileData == null)
                    {
                        return NotFound();
                    }
                    return File(file.FileData, file.DetectedFileType);
                }
                else
                {
                    if (string.IsNullOrEmpty(file.FilePath) || !System.IO.File.Exists(file.FilePath))
                    {
                        return NotFound();
                    }
                    try
                    {
                        FileStream reader = System.IO.File.OpenRead(file.FilePath);
                        return File(reader, file.DetectedFileType);
                    }
                    catch
                    {
                        return NotFound();
                    }


                }
            }

            return Forbid();
        }
    }
}
