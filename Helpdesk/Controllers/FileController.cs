using Helpdesk.Authorization;
using Helpdesk.Data;
using Helpdesk.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : DI_BaseController
    {

        public FileController(ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
            : base(dbContext, userManager, signInManager)
        { }

        private async Task<IActionResult> ReturnFile(FileUpload file)
        {
            if (file.IsDatabaseFile)
            {
                if (file.FileData == null)
                {
                    return NotFound();
                }
                return File(file.FileData, file.MIMEType);
            }
            else
            {
                if (string.IsNullOrEmpty(file.FilePath))
                {
                    return NotFound();
                }
                string filePath = await FileHelpers.GetActualFilePath(_context, file);
                if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
                {
                    return NotFound();
                }

                try
                {
                    FileStream reader = System.IO.File.OpenRead(filePath);
                    return File(reader, file.MIMEType);
                }
                catch
                {
                    return NotFound();
                }
            }
        }

        [HttpGet("download")]
        public async Task<IActionResult> Download([FromQuery] string? fileid)
        {
            if (fileid == null)
            {
                return BadRequest();
            }
            // If file allows anonymous access, return the file.
            var file = await _context.FileUploads.Where(x => x.Id == fileid).FirstOrDefaultAsync();
            // we can't return NotFound for a totally invalid request.
            // Only check for file record exists and that it allows anonymous access
            if (file != null && file.AllowUnauthenticatedAccess)
            {
                return await ReturnFile(file);
            }
            // either the file doesn't have a file record, or the file doesn't allow anonymous access
            // Load the user info so we can perform authorization checks
            await LoadControllerSettings(ViewData);
            if (_currentHelpdeskUser == null)
            {
                // This happens when a user logs in, but hasn't set up their profile yet.
                return Forbid();
            }
            if (string.IsNullOrEmpty(fileid) || file == null)
            {
                // They are authenticated and they requested an invalid or nonexistent file.
                return NotFound();
            }
            // if we make it here, they're authenticated.
            // if file allows authenticated access, return the file.
            if (file != null && file.AllowAllAuthenticatedAccess)
            {
                return await ReturnFile(file);
            }

            // perform permission validation.
            bool OwnAccess = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.FileManagerOwnAccess);
            bool AdminAccess = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.FileManagerAdminAccess);
            if (!OwnAccess && !AdminAccess)
            {
                return Forbid();
            }
            // now we can tell the user if no file was found.
            if (file == null)
            {
                return NotFound();
            }

            if (AdminAccess ||
                (OwnAccess && !string.IsNullOrEmpty(file.UploadedBy) && file.UploadedBy == _currentHelpdeskUser.IdentityUserId))
            {
                return await ReturnFile(file);
            }

            return Forbid();
        }
    }
}
