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
using System.Globalization;

namespace Helpdesk.Pages.Tickets
{
    public class IndexModel : DI_BasePageModel
    {

        public IndexModel(ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
            : base(dbContext, userManager, signInManager)
        { }

        public class TicketView
        {
            public string Id { get; set; }
            public string Title { get; set; }
            [Display(Name = "Type")]
            public string Type { get; set; }
            [Display(Name = "Submit Date")]
            public string SubmitDate { get; set; }
            public string RequesterId { get; set; }
            public string Requester { get; set; }
            public string HandlerId { get; set; }
            public string Handler { get; set; }
            public string Status { get; set; }
            public string Priority { get; set; }
            [Display(Name = "Last Update")]
            public string LastUpdate { get; set; }
            [Display(Name = "Due Date")]
            public string DueDate { get; set; }
        }

        public IList<TicketView> Tickets { get;set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadSiteSettings(ViewData);
            if (_currentHelpdeskUser == null)
            {
                // This happens when a user logs in, but hasn't set up their profile yet.
                return Forbid();
            }
            bool HasSubmitter = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.TicketSubmitter);
            bool HasHandler = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.TicketHandler);
            bool HasReviewer = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.TicketReviewer);
            if (!HasSubmitter && !HasHandler && !HasReviewer)
            {
                return Forbid();
            }
            Tickets = new List<TicketView>();

            var tickets = await _context.TicketMasters
                .Where(x => !x.TicketStatus.Archived &&
                            (x.TicketWatchers.Any(y => y.User.IdentityUserId == _currentHelpdeskUser.IdentityUserId) ||
                             x.Requester.IdentityUserId == _currentHelpdeskUser.IdentityUserId ||
                             (x.Handler != null && x.Handler.IdentityUserId == _currentHelpdeskUser.IdentityUserId)))
                .Include(x => x.TicketStatus)
                .Include(x => x.Requester)
                .Include(x => x.Handler)
                .Include(x => x.TicketType)
                .Include(x => x.TicketPriority)
                .OrderBy(x => x.CreationDate)
                .ToListAsync();
            foreach (var ticket in tickets)
            {
                Tickets.Add(new TicketView()
                {
                    Id = ticket.Id,
                    Title = ticket.Title,
                    Type = ticket.TicketType?.Name ?? "",
                    SubmitDate = ticket.CreationDate.ToString("f", CultureInfo.GetCultureInfo("en-US")),
                    Requester = ticket.Requester.DisplayName,
                    RequesterId = ticket.Requester.IdentityUserId,
                    Handler = ticket.Handler?.DisplayName ?? "",
                    HandlerId = ticket.Handler?.IdentityUserId ?? "",
                    Status = ticket.TicketStatus.Name,
                    Priority = ticket.TicketPriority.Name,
                    LastUpdate = ticket.LastUpdate?.ToString("f", CultureInfo.GetCultureInfo("en-US")) ?? "",
                    DueDate = ticket.DueDate?.ToString("f", CultureInfo.GetCultureInfo("en-US")) ?? ""
                });
            }

            return Page();
        }
    }
}
