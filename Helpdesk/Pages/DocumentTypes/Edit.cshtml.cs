using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Helpdesk.Data;
using Helpdesk.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Helpdesk.Authorization;

namespace Helpdesk.Pages.DocumentTypes
{
    public class EditModel : DI_BasePageModel
    {

        public EditModel(ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
            : base(dbContext, userManager, signInManager)
        { }

        [BindProperty]
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

            var documenttype =  await _context.DocumentTypes.FirstOrDefaultAsync(m => m.Id == id);
            if (documenttype == null)
            {
                return NotFound();
            }
            else if (documenttype.IsSystemType)
            {
                return Forbid();
            }
            DocumentType = documenttype;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
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
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var dt = await _context.DocumentTypes
                .Where(x => x.Id == DocumentType.Id)
                .FirstOrDefaultAsync();
            if (dt == null)
            {
                return NotFound();
            }
            else if (dt.IsSystemType)
            {
                return Forbid();
            }
            var existdt = await _context.DocumentTypes
                .Where(x => x.Name == DocumentType.Name)
                .FirstOrDefaultAsync();
            if (existdt != null && existdt.Id != dt.Id)
            {
                ModelState.AddModelError("DocumentType.Name", "That document type already exists.");
                return Page();
            }

            dt.Name = DocumentType.Name;
            dt.Description = DocumentType.Description;
            _context.DocumentTypes.Update(dt);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DocumentTypeExists(DocumentType.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool DocumentTypeExists(int id)
        {
          return (_context.DocumentTypes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
