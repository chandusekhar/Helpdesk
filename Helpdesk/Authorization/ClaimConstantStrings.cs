namespace Helpdesk.Authorization
{
    public static class ClaimConstantStrings
    {
        /// <summary>
        /// Allows creating a new Role. Requires RolesViewClaims, RolesEditClaims
        /// </summary>        
        public static readonly string RolesCreateNew = "Roles Create New";
        /// <summary>
        /// Allows viewing a Role's claims
        /// </summary>
        public static readonly string RolesViewClaims = "Roles View Claims";
        /// <summary>
        /// Allows Editing a Role's claims
        /// </summary>
        public static readonly string RolesEditClaims = "Roles Edit Claims";
        /// <summary>
        /// Allows deleting a role
        /// </summary>
        public static readonly string RolesDeleteRole = "Roles Delete Role";
        
        /// <summary>
        /// Allows editing sitewide configuration settings
        /// </summary>
        public static readonly string SitewideConfigurationEditor = "Sitewide Configuration Editor";

        /// <summary>
        /// Allows creating/editing/removing License types
        /// </summary>
        public static readonly string LicenseTypeAdmin = "License Type Admin";

        /// <summary>
        /// Allows creating users, resetting passwords for users, enabling/disabling users
        /// </summary>
        public static readonly string UsersAdmin = "Users Admin";

        /// <summary>
        /// Allows granting/revoking roles for users. Requires UsersAdmin to get to the page to do this
        /// </summary>
        public static readonly string UsersRolesAdmin = "Users Roles Admin";

        ///// <summary>
        ///// Allows password reset, enabling/disabling of users with privileged roles
        ///// </summary>
        //public static readonly string UsersPrivilegedAdmin = "Users Privileged Admin";

        ///// <summary>
        ///// Allows adding a privileged role to a user (super admin, for example). Requires UsersAdmin to get to the page to do this
        ///// </summary>
        //public static readonly string UsersPrivilegedRolesAdmin = "Users Privileged Roles Admin";

        /// <summary>
        /// Required to access the users screen and view basic user details. 
        /// </summary>
        public static readonly string UsersAllowReadAccess = "Users Allow Read Access";
            
    }
}
