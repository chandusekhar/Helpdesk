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

namespace Helpdesk.Pages.DocumentTypes
{
    public class DetailsModel : DI_BasePageModel
    {

        public DetailsModel(ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
            : base(dbContext, userManager, signInManager)
        { }

        public DocumentType DocumentType { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            await LoadSiteSettings(ViewData);
            if (_currentHelpdeskUser == null)
            {
                // This happens when a user logs in, but hasn't set up their profile yet.
                return Forbid();
            }
            bool HasClaim = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.DocumentTypeAdmin);
            if (!HasClaim)
            {
                return Forbid();
            }
            if (id == null || _context.DocumentTypes == null)
            {
                return NotFound();
            }

            var documenttype = await _context.DocumentTypes.FirstOrDefaultAsync(m => m.Id == id);
            if (documenttype == null)
            {
                return NotFound();
            }
            else 
            {
                DocumentType = documenttype;
            }
            return Page();
        }
    }
}
