using Helpdesk.Data;

namespace Helpdesk.Authorization
{
    public static class RightsManagement
    {
        public static async Task<bool> UserIsInRole(ApplicationDbContext context, string userId, string role)
        {

            return false;

        }

        public static async Task UserAddRole(ApplicationDbContext context, string userId, string role)
        {

        }

        public static async Task UserRemoveRole(ApplicationDbContext context, string userId, string role)
        {

        }
        
        public static async Task<IEnumerable<HelpdeskRole>> UserListRoles(ApplicationDbContext context, string userId)
        {
            return new List<HelpdeskRole>();
        }

        public static async Task<IEnumerable<HelpdeskClaim>> UserListClaims(ApplicationDbContext context, string userId)
        {
            return new List<HelpdeskClaim>();
        }

        public static async Task<bool> UserHasClaim(ApplicationDbContext context, HelpdeskUser user, string role)
        {

            return false;

        }

        public static async Task<IEnumerable<HelpdeskRole>> GetAllRoles(ApplicationDbContext context)
        {
            return new List<HelpdeskRole>();
        }

        public static async Task<IEnumerable<HelpdeskClaim>> GetAllClaims(ApplicationDbContext context)
        {
            return new List<HelpdeskClaim>();
        }

        public static async Task<UserAuthorizeContext> GetUserAuthContext(ApplicationDbContext context, string userId)
        {

            return new UserAuthorizeContext();
        }

        

    }
}
