namespace Helpdesk.Authorization
{
    public static class ClaimConstantStrings
    {
        // Allows creating a new Role. Requires RolesViewClaims, RolesEditClaims.
        public static readonly string RolesCreateNew = "Roles Create New";
        // Allows viewing a Role's claims.
        public static readonly string RolesViewClaims = "Roles View Claims";
        // Allows Editing a Role's claims.
        public static readonly string RolesEditClaims = "Roles Edit Claims";
        // Allows deleting a role.
        public static readonly string RolesDeleteRole = "Roles Delete Role";
        
        // Allows editing sitewide configuration settings
        public static readonly string SitewideConfigurationEditor = "Sitewide Configuration Editor";

        // Allows creating users, resetting passwords for users, enabling/disabling users.
        public static readonly string UsersAdmin = "Users Admin";

        // Allows granting/revoking roles for users. Requires UsersAdmin to get to the page to do this.
        public static readonly string UsersRolesAdmin = "Users Roles Admin";

        // Allows password reset, enabling/disabling of users with privileged roles. 
        public static readonly string UsersPrivilegedAdmin = "Users Privileged Admin";

        // Allows adding a privileged role to a user (super admin, for example). Requires UsersAdmin to get to the page to do this.
        public static readonly string UsersPrivilegedRolesAdmin = "Users Privileged Roles Admin";

    }
}
