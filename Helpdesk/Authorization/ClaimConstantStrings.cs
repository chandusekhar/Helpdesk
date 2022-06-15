namespace Helpdesk.Authorization
{
    public static class ClaimConstantStrings
    {
        /// <summary>
        /// Allows creating, editing, and deleting roles.
        /// </summary>        
        public static readonly string HelpdeskRolesAdmin = "Roles Administrator";
        
        /// <summary>
        /// Allows editing sitewide configuration settings
        /// </summary>
        public static readonly string SitewideConfigurationEditor = "Sitewide Configuration Editor";

        /// <summary>
        /// Allows creating/editing/removing License types
        /// </summary>
        public static readonly string LicenseTypeAdmin = "License Type Admin";

        /// <summary>
        /// Allows creating/editing/removing groups
        /// </summary>
        public static readonly string GroupAdmin = "Group Admin";

        /// <summary>
        /// Allows creating users, resetting passwords for users, enabling/disabling users
        /// </summary>
        public static readonly string UsersAdmin = "Users Admin";

        /// <summary>
        /// Allows granting/revoking roles for users. Requires UsersAdmin to get to the page to do this
        /// </summary>
        public static readonly string UsersRolesAdmin = "Users Roles Admin";

        /// <summary>
        /// Required to access the users screen and view basic user details. 
        /// </summary>
        public static readonly string UsersAllowReadAccess = "Users Allow Read Access";

        /// <summary>
        /// In User Details page, this claim will allow them to view the license product code
        /// Without this claim, they can see that a license is assigned to a user, but the product code is not shown
        /// </summary>
        public static readonly string UsersAllowReadLicenseProductCode = "Users Allow Read License Product Code";

        /// <summary>
        /// Allows importing and exporting data users/assets.
        /// </summary>
        public static readonly string ImportExport = "Import and Export";

        /// <summary>
        /// Allows creating/editing/removing asset options like Asset Types, Manufacturers, Models, Vendors.
        /// </summary>
        public static readonly string AssetOptionsEditor = "Asset Options Editor";

        /// <summary>
        /// Allows editing and creating assets, setting properties, assigning to users, and assigning licenses.
        /// </summary>
        public static readonly string AssetsManager = "Assets Manager";

        /// <summary>
        /// Required to access the assets screen and view basic asset details. 
        /// </summary>
        public static readonly string AssetsAllowReadAccess = "Assets Allow Read Access";

        /// <summary>
        /// Allows viewing license product codes for assets on the details page.
        /// </summary>
        public static readonly string AssetsAllowReadLicenseProductCode = "Assets Allow Read License Product Code";

        /// <summary>
        /// Allows uploading, downloading, or deleting uploaded files using File Manager.
        /// </summary>
        public static readonly string FileManagerAdminAccess = "File Manager Admin Access";

        /// <summary>
        /// Allows downloading and deleting documents owned by the user in File Manager.
        /// </summary>
        public static readonly string FileManagerOwnAccess = "File Manager Own Access";

        /// <summary>
        /// Allows creating, editing, or deleting document types for File Uplaods
        /// </summary>
        public static readonly string DocumentTypeAdmin = "Document Type Admin";
    }
}
