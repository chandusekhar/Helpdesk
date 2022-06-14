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
using Microsoft.EntityFrameworkCore;

namespace Helpdesk.Pages.DocumentTypes
{
    public class CreateModel : DI_BasePageModel
    {

        public CreateModel(ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
            : base(dbContext, userManager, signInManager)
        { }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public DocumentType DocumentType { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
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
            if (!ModelState.IsValid || _context.DocumentTypes == null || DocumentType == null)
            {
                return Page();
            }

            var dt = await _context.DocumentTypes
                .Where(x => x.Name == DocumentType.Name)
                .FirstOrDefaultAsync();
            if (dt != null)
            {
                ModelState.AddModelError("DocumentType.Name", "This document type already exists.");
                return Page();
            }
            _context.DocumentTypes.Add(new DocumentType()
            {
                Name = DocumentType.Name,
                Description = DocumentType.Description,
                IsSystemType = false
            });
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
