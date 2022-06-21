using Helpdesk.Data;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk.Authorization
{
    public static class RightsManagement
    {
        public static async Task<List<HelpdeskRole>> UserRolesAssigned(ApplicationDbContext context, string userId)
        {
            var userRoles = new List<HelpdeskRole>();
            var dbRoles = await context.HelpdeskUsers
                .Where(x => x.IdentityUserId == userId)
                .Include(y => y.Roles)
                .FirstOrDefaultAsync();
            if (dbRoles != null)
            {
                foreach (var r in dbRoles.Roles)
                {
                    userRoles.Add(r);
                } 
            }
            return userRoles;
        }

        public static async Task<bool> UserHasSuperAdmin(ApplicationDbContext context, string userId)
        {
            var huser = await context.HelpdeskUsers
                .Where(x => x.IdentityUserId == userId && 
                            x.Roles.Any(y => y.IsSuperAdmin))
                .FirstOrDefaultAsync();
            if (huser == null)
            {
                return false;
            }

            return huser.IdentityUserId == userId;
        }

        public static async Task<bool> UserIsInRole(ApplicationDbContext context, string userId, string role)
        {
            var huser = await context.HelpdeskUsers
                .Where(x => x.IdentityUserId == userId)
                .Include(x => x.Roles)
                .FirstOrDefaultAsync();
            if (huser == null)
            {
                return false;
            }
            return huser.Roles.Any(x => x.Name == role);
        }

        public static async Task<List<HelpdeskUser>> GetUsersWithRole(ApplicationDbContext context, string role)
        {
            var users = await context.HelpdeskUsers
                .Where(x => x.Roles.Any(y => y.Name == role))
                .ToListAsync();
            if (users == null || users.Count == 0)
            {
                return new List<HelpdeskUser>();
            }
            else
            {
                return users;
            }
        }

        public static async Task<bool> UserHasClaim(ApplicationDbContext context, string userId, string claimName)
        {
            var huser = await context.HelpdeskUsers
                .Where(x => x.IdentityUserId == userId)
                .Include(x => x.Roles)
                .ThenInclude(y => y.Claims.Where(z => z.Name == claimName))
                .FirstOrDefaultAsync();
            if (huser == null)
            {
                return false;
            }
            return huser.Roles.Any(x => x.Claims.Any(y => y.Name == claimName));
        }

        public static async Task<bool> UserAddRole(ApplicationDbContext context, string userId, string roleName)
        {
            var huser = await context.HelpdeskUsers
                .Where(x => x.IdentityUserId == userId)
                .Include(x => x.Roles.Where(y => y.Name == roleName))
                .FirstOrDefaultAsync();
            if (huser == null)
            {
                return false;
            }
            if (huser.Roles.Any(x => x.Name == roleName))
            {
                return true;
            }
            var role = await context.HelpdeskRoles.Where(x => x.Name == roleName).FirstOrDefaultAsync();
            if (role == null)
            {
                return false;
            }
            huser.Roles.Add(role);
            await context.SaveChangesAsync();
            return true;
        }

        public static async Task<bool> UserRemoveRole(ApplicationDbContext context, string userId, string roleName)
        {
            var huser = await context.HelpdeskUsers
                .Where(x => x.IdentityUserId == userId)
                .Include(x => x.Roles.Where(y => y.Name == roleName))
                .FirstOrDefaultAsync();
            if (huser == null)
            {
                return false;
            }
            if (huser.Roles.Count() == 0)
            {
                return true;
            }
            huser.Roles.Remove(huser.Roles.First());
            await context.SaveChangesAsync();
            return true;
        }

        //    public static async Task<List<HelpdeskRole>> UserListRoles(ApplicationDbContext context, string userId)
        //    {
        //        return new List<HelpdeskRole>();
        //    }

        //    public static async Task<List<HelpdeskClaim>> UserListClaims(ApplicationDbContext context, string userId)
        //    {
        //        return new List<HelpdeskClaim>();
        //    }

        public static async Task<List<HelpdeskRole>> GetAllRoles(ApplicationDbContext context)
        {
            return await context.HelpdeskRoles.ToListAsync();
        }

        //    public static async Task<UserAuthorizeContext> GetUserAuthContext(ApplicationDbContext context, string userId)
        //    {
        //        return new UserAuthorizeContext();
        //    }

        public static async Task<List<HelpdeskClaim>> GetAllClaims(ApplicationDbContext context)
        {
            return await context.HelpdeskClaims.ToListAsync();
        }

        public static async Task<HelpdeskRole?> GetRoleIfExists(ApplicationDbContext context, string roleName)
        {
            return await context.HelpdeskRoles.Where(x => x.Name == roleName).FirstOrDefaultAsync();
        }

        public static async Task<HelpdeskRole> CreateRole(ApplicationDbContext context, string roleName, string roleDescription, bool isPrivileged, bool isSuperAdmin)
        {
            var newrole = new HelpdeskRole()
            {
                Name = roleName,
                Description = roleDescription,
                IsPrivileged = isPrivileged,
                IsSuperAdmin = isSuperAdmin
            };
            context.HelpdeskRoles.Add(newrole);
            await context.SaveChangesAsync();
            return newrole;
        }

        public static async Task<List<HelpdeskClaim>> GetRoleClaims(ApplicationDbContext context, string roleName)
        {
            var role = await context.HelpdeskRoles.Where(x => x.Name == roleName).Include(x => x.Claims).FirstOrDefaultAsync();
            if (role == null)
            {
                return new List<HelpdeskClaim>();
            }
            return role.Claims.ToList();
        }

        public static async Task<HelpdeskClaim> CreateClaim(ApplicationDbContext context, string claimName, string claimDescription, bool isSystemClaim)
        {
            var newclaim = new HelpdeskClaim()
            {
                Name = claimName,
                Description = claimDescription,
                IsSystemType = isSystemClaim
            };
            context.HelpdeskClaims.Add(newclaim);
            await context.SaveChangesAsync();
            return newclaim;
        }

        public static async Task<bool> AddClaimToRole(ApplicationDbContext context, string roleName, string claimName)
        {
            var role = await context.HelpdeskRoles.Where(x => x.Name == roleName).Include(x => x.Claims).FirstOrDefaultAsync();
            var claim = await context.HelpdeskClaims.Where(x => x.Name == claimName).FirstOrDefaultAsync();
            if (role == null || claim == null)
            {
                return false;
            }
            if (role.Claims.Contains(claim))
            {
                return true;
            }
            role.Claims.Add(claim);
            await context.SaveChangesAsync();
            return true;
        }

        public static async Task<bool> RemoveClaimFromRole(ApplicationDbContext context, string roleName, string claimName)
        {
            var role = await context.HelpdeskRoles.Where(x => x.Name == roleName).Include(x => x.Claims).FirstOrDefaultAsync();
            var claim = await context.HelpdeskClaims.Where(x => x.Name == claimName).FirstOrDefaultAsync();
            if (role == null || claim == null)
            {
                return true;
            }
            if (!role.Claims.Contains(claim))
            {
                return true;
            }
            role.Claims.Remove(claim);
            await context.SaveChangesAsync();
            return true;
        }

        public static async Task RemoveAllRolesFromUser(ApplicationDbContext context, string userId)
        {
            var huser = await context.HelpdeskUsers
                .Where(x => x.IdentityUserId == userId)
                .Include(x => x.Roles)
                .FirstOrDefaultAsync();
            if (huser == null)
            {
                return;
            }
            if (huser.Roles.Count() == 0)
            {
                return;
            }
            foreach (var role in huser.Roles)
            {
                huser.Roles.Remove(role);
            }
            await context.SaveChangesAsync();
            return;
        }

    }
}
