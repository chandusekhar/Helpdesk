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
using System.ComponentModel.DataAnnotations;
using Helpdesk.Authorization;

namespace Helpdesk.Pages.SiteSettings
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
            [Required]
            public int Id { get; set; }

            [Required]
            public string Category { get; set; } = String.Empty;

            [Required]
            public string Key { get; set; } = String.Empty;

            public string Value { get; set; } = String.Empty;
        }

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (!await LoadSiteSettings(ViewData))
            {
                return RedirectToPage("/Index");
            }
            if (_currentHelpdeskUser == null)
            {
                return Forbid();
            }
            bool HasClaim = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.SitewideConfigurationEditor);
            if (!HasClaim)
            {
                return Forbid();
            }
            if (id == null || _context.ConfigOpts == null)
            {
                return NotFound();
            }

            var configopt =  await _context.ConfigOpts.FirstOrDefaultAsync(m => m.Id == id);
            if (configopt == null)
            {
                return NotFound();
            }
            Input = new InputModel()
            {
                Id = configopt.Id,
                Category = configopt.Category,
                Key = configopt.Key,
                Value = configopt.Value
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!await LoadSiteSettings(ViewData))
            {
                return RedirectToPage("/Index");
            }
            if (_currentHelpdeskUser == null)
            {
                return Forbid();
            }
            bool HasClaim = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.SitewideConfigurationEditor);
            if (!HasClaim)
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var opt = await _context.ConfigOpts.Where(x => 
                    x.Id == Input.Id && 
                    x.Category == Input.Category && 
                    x.Key == Input.Key)
                .FirstOrDefaultAsync();

            if (opt == null)
            {
                return NotFound();
            }

            if (opt.Value != Input.Value)
            {
                opt.Value = Input.Value;
                _context.ConfigOpts.Update(opt);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage("./Index");
        }
    }
}
