using Helpdesk.Authorization;
using Helpdesk.Data;
using Helpdesk.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Pages.People
{
    public class AddTeamModel : DI_BasePageModel
    {
        public AddTeamModel(ApplicationDbContext dbContext,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
            : base(dbContext, userManager, signInManager)
        { }

        public class InputModel
        {
            [Required]
            public string UserId { get; set; }
            [Required]
            public string DisplayName { get; set; }
            
            public List<SelectedUser>? SelectedUsers { get; set; }
            public List<SelectedResp>? SelectedResps { get; set; }
        }

        public class SelectedUser
        {
            [Required]
            public string UserId { get; set; }
            [Required]
            public string DisplayName { get; set; }
        }

        public class SelectedResp
        {
            [Required]
            public int RespId { get; set; }
            [Required]
            public string Display { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [BindProperty(Name = "userselector")]
        public string? AddUserId { get; set; }

        [BindProperty(Name = "respselector")]
        public string? AddRespId { get; set; }

        public class SelectableUser
        {
            public string UserId { get; set; }
            public string DisplayName { get; set; }

        }


        public class SelectableResp
        {
            public int RespId { get; set; }
            public string Display { get; set; }
        }

        public List<SelectableUser> UserList { get; set; }

        public List<SelectableResp> RespList { get; set; }

        public async Task<IActionResult> OnGetAsync([FromQuery] string? id)
        {
            await LoadSiteSettings(ViewData);
            if (_currentHelpdeskUser == null)
            {
                // This happens when a user logs in, but hasn't set up their profile yet.
                return Forbid();
            }
            bool HasClaim = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.UsersAdmin);
            if (!HasClaim)
            {
                return Forbid();
            }
            if (id == null || _context.HelpdeskUsers == null || _userManager == null)
            {
                return NotFound();
            }

            var huser = await _context.HelpdeskUsers.Where(x => x.IdentityUserId == id).FirstOrDefaultAsync();
            if (huser == null)
            {
                return NotFound();
            }


            Input = new InputModel()
            {
                UserId = huser.IdentityUserId,
                DisplayName = huser.DisplayName,
                SelectedUsers = new List<SelectedUser>(),
                SelectedResps = new List<SelectedResp>()
            };
            await BuildUserList();
            await BuildRespList();
            return Page();

        }

        private async Task BuildUserList()
        {
            if (Input.SelectedUsers == null)
            {
                Input.SelectedUsers = new List<SelectedUser>();
            }
            UserList = new List<SelectableUser>();
            var hUsers = await _context.HelpdeskUsers.OrderBy(x => x.DisplayName).ToListAsync();
            var iUsers = await _context.Users.ToListAsync();
            foreach (var h in hUsers)
            {
                if (!iUsers.Where(x => x.Id == h.IdentityUserId).Any())
                {
                    continue;
                }
                if (Input.SelectedUsers.Where(x => x.UserId == h.IdentityUserId).Any())
                {
                    continue;
                }
                if (Input.UserId == h.IdentityUserId)
                {
                    continue;
                }
                UserList.Add(new SelectableUser()
                {
                    UserId = h.IdentityUserId,
                    DisplayName  = h.DisplayName,
                });
            }
        }

        private async Task BuildRespList()
        {
            if (Input.SelectedResps == null)
            {
                Input.SelectedResps = new List<SelectedResp>();
            }
            RespList = new List<SelectableResp>();
            var rsps = await _context.SupervisorResponsibilities.OrderBy(x => x.Name).ToListAsync();
            foreach (var r in rsps)
            {
                if (Input.SelectedResps.Where(x => x.RespId == r.Id).Any())
                {
                    continue;
                }
                RespList.Add(new SelectableResp()
                {
                    RespId = r.Id,
                    Display = string.Format("{0} ({1})", r.Name, r.Description)
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> OnPostAsync()
        {
            await LoadSiteSettings(ViewData);
            if (_currentHelpdeskUser == null)
            {
                // This happens when a user logs in, but hasn't set up their profile yet.
                return Forbid();
                // For some pages, it might make sense to redirect to the account profile page so they can immediately enter their details.
                //return RedirectToPage("/Identity/Account/Manage");
            }
            bool HasClaim = await RightsManagement.UserHasClaim(_context, _currentHelpdeskUser.IdentityUserId, ClaimConstantStrings.UsersAdmin);
            if (!HasClaim)
            {
                return Forbid();
            }
            if (!ModelState.IsValid || _context.HelpdeskUsers == null || _userManager == null)
            {
                return NotFound();
            }

            if (Input.SelectedUsers == null)
            {
                Input.SelectedUsers = new List<SelectedUser>();
            }
            if (Input.SelectedResps == null)
            {
                Input.SelectedResps = new List<SelectedResp>();
            }

            if (!string.IsNullOrEmpty(AddUserId))
            {
                var nUser = await _context.HelpdeskUsers.Where(x => x.IdentityUserId == AddUserId).FirstOrDefaultAsync();
                if (nUser != null &&
                    nUser.IdentityUserId != Input.UserId &&
                    !Input.SelectedUsers.Where(x => x.UserId == nUser.IdentityUserId).Any())
                {
                    Input.SelectedUsers.Add(new SelectedUser()
                    {
                        UserId = nUser.IdentityUserId,
                        DisplayName = nUser.DisplayName
                    });
                }
            }

            if (!string.IsNullOrEmpty(AddRespId))
            {
                int rid;
                if (Int32.TryParse(AddRespId, out rid))
                {
                    var resp = await _context.SupervisorResponsibilities.Where(x => x.Id == rid).FirstOrDefaultAsync();
                    if (resp != null)
                    {
                        Input.SelectedResps.Add(new SelectedResp()
                        {
                            RespId = rid,
                            Display = string.Format("{0} ({1})", resp.Name, resp.Description)
                        });
                    }
                }
            }

            //if (string.IsNullOrEmpty(AddUserId) && string.IsNullOrEmpty(AddRespId))
            //{
            //    // save changes to the database.
            //    if (Input.SelectedUsers != null && Input.SelectedUsers.Count() > 0 &&
            //        Input.SelectedResps != null && Input.SelectedResps.Count() > 0)
            //    {
            //        var hUser = await _context.HelpdeskUsers
            //            .Where(x => x.IdentityUserId == Input.UserId)
            //            .FirstOrDefaultAsync();
            //        if (hUser == null)
            //        {
            //            return RedirectToPage("./People/Index");
            //        }
            //        Dictionary<int, SupervisorResponsibility> RespCache = new Dictionary<int, SupervisorResponsibility>();
            //        foreach (var user in Input.SelectedUsers) 
            //        {
            //            var selUsr = await _context.HelpdeskUsers
            //                .Where(x => x.IdentityUserId == user.UserId)
            //                .FirstOrDefaultAsync();
            //            if (selUsr == null)
            //            {
            //                continue;
            //            }
            //            var seliUsr = await _context.Users
            //                .Where(x => x.Id == selUsr.IdentityUserId)
            //                .FirstOrDefaultAsync();
            //            if (seliUsr == null)
            //            {
            //                continue;
            //            }
            //            foreach (var rsp in Input.SelectedResps)
            //            {
            //                SupervisorResponsibility? dbResp;
            //                if (!RespCache.TryGetValue(rsp.RespId, out dbResp))
            //                {
            //                    var selRsp = await _context.SupervisorResponsibilities
            //                        .Where(x => x.Id == rsp.RespId)
            //                        .FirstOrDefaultAsync();
            //                    if (selRsp == null)
            //                    {
            //                        continue;
            //                    }
            //                    dbResp = selRsp;
            //                    RespCache.Add(rsp.RespId, dbResp);
            //                }
            //                var existTeam = await _context.TeamMembers
            //                    .Where(x => x.Supervisor.IdentityUserId == Input.UserId &&
            //                                x.Subordinate.IdentityUserId == selUsr.IdentityUserId)
            //                    .Include(y => y.SupervisorResponsibilities)
            //                    .FirstOrDefaultAsync();

            //                if (existTeam == null)
            //                {
            //                    existTeam = new TeamMember()
            //                    {
            //                        Supervisor = hUser,
            //                        Subordinate = selUsr
            //                    };
            //                    _context.TeamMembers.Add(existTeam);
            //                    await _context.SaveChangesAsync();                                
            //                }
            //                if (!existTeam.SupervisorResponsibilities.Contains(dbResp))
            //                {
            //                    existTeam.SupervisorResponsibilities.Add(dbResp);
            //                    await _context.SaveChangesAsync();
            //                }
            //            }
            //        }
            //    }
            //}

            await BuildUserList();
            await BuildRespList();

            return Page();
        }
    }
}
