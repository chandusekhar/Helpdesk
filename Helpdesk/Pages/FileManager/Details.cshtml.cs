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
using System.ComponentModel.DataAnnotations;
using Helpdesk.Authorization;

namespace Helpdesk.Pages.FileManager
{
    public class DetailsModel : DI_BasePageModel
    {

        public DetailsModel(ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
            : base(dbContext, userManager, signInManager)
        { }
        public class FileModel
        {
            public string Id { get; set; } = string.Empty!;
            [Display(Name = "File Name")]
            public string FileName { get; set; } = string.Empty!;
            [Display(Name = "Uploader")]
            public string? Uploader { get; set; } = string.Empty!;
            public string Date { get; set; } = string.Empty!;
            public string Length { get; set; } = string.Empty!;
            public string Type { get; set; } = string.Empty!;
            public string Location { get; set; } = string.Empty!;
            public string Permissions { get; set; } = string.Empty!;
        }

        public FileModel File { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(string id)
        {
            await LoadSiteSettings(ViewData);
            if (_currentHelpdeskUser == null)
            {
                // This happens when a user logs in, but hasn't set up their profile yet.
                return Forbid();
                // For some pages, it might make sense to redirect to the account profile page so they can immediately enter their details.
                //return RedirectToPage("/Identity/Account/Manage");
            }
            bool OwnAccess = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.FileManagerOwnAccess);
            bool AdminAccess = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.FileManagerAdminAccess);
            if (!OwnAccess && !AdminAccess)
            {
                return Forbid();
            }
            if (id == null || _context.FileUploads == null)
            {
                return NotFound();
            }

            var fileupload = await _context.FileUploads.FirstOrDefaultAsync(m => m.Id == id);
            if (fileupload == null)
            {
                return NotFound();
            }
            if (AdminAccess || (OwnAccess && !string.IsNullOrEmpty(fileupload.UploadedBy) && fileupload.UploadedBy == _currentIdentityUser?.Id))
            {
                string uploader = "";
                if (fileupload.UploadedBy == _currentHelpdeskUser?.IdentityUserId)
                {
                    uploader = _currentHelpdeskUser?.DisplayName ?? "";
                }
                else
                {
                    var huser = await _context.HelpdeskUsers
                        .Where(x => x.IdentityUserId == fileupload.UploadedBy)
                        .FirstOrDefaultAsync();
                    if (huser != null)
                    {
                        uploader = huser.DisplayName;
                    }
                }
                string? flocation;
                if (fileupload.IsDatabaseFile)
                {
                    flocation = "Database";
                }
                else
                {
                    string physicalPath = await FileHelpers.GetActualFilePath(_context, fileupload);
                    if (string.IsNullOrEmpty(physicalPath))
                    {
                        flocation = "Invalid File";
                    }
                    else if (!System.IO.File.Exists(physicalPath))
                    {
                        flocation = "Missing File";
                    }
                    else
                    {
                        flocation = fileupload.FilePath ?? "Invalid File";
                    }
                }

                string permission = "Admin & Owner Only";
                if (fileupload.AllowUnauthenticatedAccess)
                {
                    permission = "Allow Anonymous Access.";
                }
                else if (fileupload.AllowAllAuthenticatedAccess)
                {
                    permission = "Authenticated User Access";
                }

                File = new FileModel()
                {
                    Id = fileupload.Id,
                    Date = fileupload.WhenUploaded.ToLongDateString(),
                    FileName = fileupload.OriginalFileName,
                    Length = FileHelpers.FormatSize(fileupload.FileLength),
                    Type = fileupload.MIMEType,
                    Uploader = uploader,
                    Location = flocation,
                    Permissions = permission
                };
            }
            else
            {
                return Forbid();
            }

            return Page();
        }
    }
}
