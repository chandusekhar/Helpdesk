using Helpdesk.Data;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk.Authorization
{
    public static class RightsManagement
    {
        //    public static async Task<bool> UserIsInRole(ApplicationDbContext context, string userId, string role)
        //    {

        //        return false;

        //    }

        //    public static async Task UserAddRole(ApplicationDbContext context, string userId, string role)
        //    {

        //    }

        //    public static async Task UserRemoveRole(ApplicationDbContext context, string userId, string role)
        //    {

        //    }

        //    public static async Task<List<HelpdeskRole>> UserListRoles(ApplicationDbContext context, string userId)
        //    {
        //        return new List<HelpdeskRole>();
        //    }

        //    public static async Task<List<HelpdeskClaim>> UserListClaims(ApplicationDbContext context, string userId)
        //    {
        //        return new List<HelpdeskClaim>();
        //    }

        //    public static async Task<bool> UserHasClaim(ApplicationDbContext context, HelpdeskUser user, string role)
        //    {

        //        return false;

        //    }

        //    public static async Task<List<HelpdeskRole>> GetAllRoles(ApplicationDbContext context)
        //    {
        //        return new List<HelpdeskRole>();
        //    }



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

        public static async Task<HelpdeskRole> CreateRole(ApplicationDbContext context, string roleName, string roleDescription)
        {
            var newrole = new HelpdeskRole()
            {
                Name = roleName,
                Description = roleDescription
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

        public static async Task<HelpdeskClaim> CreateClaim(ApplicationDbContext context, string claimName, string claimDescription)
        {
            var newclaim = new HelpdeskClaim()
            {
                Name = claimName,
                Description = claimDescription
            };
            context.HelpdeskClaims.Add(newclaim);
            await context.SaveChangesAsync();
            return newclaim;
        }

        public static async Task<bool> AddClaimToRoll(ApplicationDbContext context, string roleName, string claimName)
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
    }
}
