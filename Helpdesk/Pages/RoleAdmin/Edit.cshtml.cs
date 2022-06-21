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
using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Pages.RoleAdmin
{
    public class EditModel : DI_BasePageModel
    {
        public EditModel(ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
            : base(dbContext, userManager, signInManager)
        { }

        public class InputModel
        {
            [Key]
            public int Id { get; set; }
            [Required]
            public string Name { get; set; }
            [Required]
            public string Description { get; set; }
            [Required]
            [Display(Name="Privileged Role")]
            public bool IsPrivileged { get; set; }
            [Required]
            [Display(Name="Super Admin")]
            public bool IsSuperAdmin { get; set; }
            [Required]
            public RoleClaim[] RoleClaims { get; set; }
        }

        public class RoleClaim
        {
            [Required]
            [Display(Name = "Granted")]
            public bool IsGranted { get; set; }
            [Required]
            public bool WasGranted { get; set; }
            [Required]
            public string Claim { get; set; }
            [Required]
            public string Description { get; set; }
        }

        [BindProperty]
        public InputModel HelpdeskRole { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            await LoadSiteSettings(ViewData);
            if (_currentHelpdeskUser == null)
            {
                // This happens when a user logs in, but hasn't set up their profile yet.
                return Forbid();
            }
            bool HasClaim = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.HelpdeskRolesAdmin);
            if (!HasClaim)
            {
                return Forbid();
            }
            if (id == null || _context.HelpdeskRoles == null)
            {
                return NotFound();
            }

            var helpdeskrole =  await _context.HelpdeskRoles.FirstOrDefaultAsync(m => m.Id == id);
            if (helpdeskrole == null)
            {
                return NotFound();
            }

            HelpdeskRole = new InputModel()
            {
                Id = helpdeskrole.Id,
                Name = helpdeskrole.Name,
                Description = helpdeskrole.Description,
                IsPrivileged = helpdeskrole.IsPrivileged,
                IsSuperAdmin = helpdeskrole.IsSuperAdmin
            };
            var allClaims = await RightsManagement.GetAllClaims(_context);
            var heldClaims = await RightsManagement.GetRoleClaims(_context, helpdeskrole.Name);
            List<RoleClaim> claims = new List<RoleClaim>();
            foreach (var claim in allClaims.OrderBy(x => x.Name))
            {
                bool held = heldClaims.Where(x => x.Name == claim.Name).Any();
                claims.Add(new RoleClaim()
                {
                    Claim = claim.Name,
                    Description = claim.Description,
                    IsGranted = held,
                    WasGranted = held
                });
            }
            HelpdeskRole.RoleClaims = claims.ToArray();
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
            bool HasClaim = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.HelpdeskRolesAdmin);
            if (!HasClaim)
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var role = await _context.HelpdeskRoles.Where(x => x.Id == HelpdeskRole.Id).FirstOrDefaultAsync();
            if (role == null)
            {
                return NotFound();
            }
            if (role.Name != HelpdeskRole.Name)
            {
                bool roleexist = await _context.HelpdeskClaims.Where(x => x.Name == HelpdeskRole.Name).AnyAsync();
                if (roleexist)
                {
                    ModelState.AddModelError("HelpdeskRole.Name", "This role name already exists.");
                    return Page();
                }
            }

            if (role.IsSuperAdmin && !HelpdeskRole.IsSuperAdmin)
            {
                int count = await _context.HelpdeskRoles.Where(x => x.IsSuperAdmin).CountAsync();
                if (count == 1)
                {
                    ModelState.AddModelError("HelpdeskRole.IsSuperAdmin", "You can't remove the last Super Admin.");
                    return Page();
                }
            }

            role.Name = HelpdeskRole.Name;
            role.Description = HelpdeskRole.Description;
            role.IsPrivileged = HelpdeskRole.IsPrivileged;
            role.IsSuperAdmin = HelpdeskRole.IsSuperAdmin;
            _context.HelpdeskRoles.Update(role);
            await _context.SaveChangesAsync();

            for (int i = 0; i < HelpdeskRole.RoleClaims.Length; i++)
            { 
                if (HelpdeskRole.RoleClaims[i].IsGranted && !HelpdeskRole.RoleClaims[i].WasGranted)
                {
                    await RightsManagement.AddClaimToRole(_context, role.Name, HelpdeskRole.RoleClaims[i].Claim);
                }
                else if (!HelpdeskRole.RoleClaims[i].IsGranted && HelpdeskRole.RoleClaims[i].WasGranted)
                {
                    await RightsManagement.RemoveClaimFromRole(_context, role.Name, HelpdeskRole.RoleClaims[i].Claim);
                }
            }
            return RedirectToPage("./Index");
        }

        private bool HelpdeskRoleExists(int id)
        {
          return (_context.HelpdeskRoles?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
