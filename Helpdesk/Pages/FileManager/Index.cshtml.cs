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

namespace Helpdesk.Pages.FileManager
{
    public class IndexModel : DI_BasePageModel
    {
        public IndexModel(ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
            : base(dbContext, userManager, signInManager)
        { }

        public class InputModel
        {
            public string Id { get; set; } = string.Empty!;
            [Display(Name = "File Name")]
            public string FileName { get; set; } = string.Empty!;
            [Display(Name = "Uploader")]
            public string? Uploader { get; set; } = string.Empty!;
            public string Date { get; set; } = string.Empty!;
            public string Length { get; set; } = string.Empty!;
            public string Type { get; set; } = string.Empty!;
        }

        public IList<InputModel> Input { get;set; } = default!;

        public Dictionary<string, string> UserList = new Dictionary<string, string>();

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
            bool OwnAccess = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.FileManagerOwnAccess);
            bool AdminAccess = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.FileManagerAdminAccess);

            if (!OwnAccess && !AdminAccess)
            {
                return Forbid();
            }

            UserList.Add("", "");

            List<FileUpload> files;
            if (AdminAccess)
            {
                files = await _context.FileUploads
                    .Include(y => y.DocumentType)
                    .ToListAsync();
                var ulist = await _context.HelpdeskUsers.Select(x => new { x.IdentityUserId, x.DisplayName }).ToListAsync();
                foreach (var u in ulist)
                {
                    UserList.Add(u.IdentityUserId, u.DisplayName);
                }
            }
            else
            {
                if (_currentIdentityUser == null)
                {
                    return NotFound();
                }
                files = await _context.FileUploads
                    .Where(x => x.UploadedBy == _currentIdentityUser.Id)
                    .Include(y => y.DocumentType)
                    .ToListAsync();
                UserList.Add(_currentHelpdeskUser.IdentityUserId, _currentHelpdeskUser.DisplayName);
            }

            Input = new List<InputModel>();
            foreach (var f in files)
            {
                Input.Add(new InputModel()
                {
                    Id = f.Id,
                    FileName = f.OriginalFileName,
                    Uploader = f.UploadedBy ?? "",
                    Date = f.WhenUploaded.ToLongDateString(),
                    Length = FileHelpers.FormatSize(f.FileLength),
                    Type = f.DocumentType?.Name ?? ""
                });
            }
            return Page();
        }

    }


}
