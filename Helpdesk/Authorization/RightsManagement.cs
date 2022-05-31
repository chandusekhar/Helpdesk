using Helpdesk.Data;

namespace Helpdesk.Authorization
{
    public static class RightsManagement
    {
        public static bool UserIsInRole(ApplicationDbContext context, string userId, string role)
        {

            return false;

        }

        public static void UserAddRole(ApplicationDbContext context, string userId, string role)
        {

        }

        public static void UserRemoveRole(ApplicationDbContext context, string userId, string role)
        {

        }
        
        public static IEnumerable<HelpdeskRole> UserListRoles(ApplicationDbContext context, string userId)
        {
            return new List<HelpdeskRole>();
        }

        public static IEnumerable<HelpdeskClaim> UserListClaims(ApplicationDbContext context, string userId)
        {
            return new List<HelpdeskClaim>();
        }

        public static bool UserHasClaim(ApplicationDbContext context, HelpdeskUser user, string role)
        {

            return false;

        }

        public static IEnumerable<HelpdeskRole> GetAllRoles(ApplicationDbContext context)
        {
            return new List<HelpdeskRole>();
        }

        public static IEnumerable<HelpdeskClaim> GetAllClaims(ApplicationDbContext context)
        {
            return new List<HelpdeskClaim>();
        }


    }
}
