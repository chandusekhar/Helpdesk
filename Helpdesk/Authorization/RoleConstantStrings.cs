namespace Helpdesk.Authorization
{
    public static class RoleConstantStrings
    {
        /// <summary>
        /// Super Admin can do anything on the site
        /// </summary>
        public static readonly string SuperAdmin = "Super Admin";
        /// <summary>
        /// User Admin can create users, reset user passwords and security options, and enable/disable accounts.
        /// </summary>
        public static readonly string UserAdmin = "User Admin";
        /// <summary>
        /// Grants readonly access to users to view properties and assignments.
        /// </summary>
        public static readonly string UserReviewer = "User Reviewer";
        /// <summary>
        /// Ticket Submitter can create new tickets and update info on their own tickets.
        /// </summary>
        public static readonly string TicketSubmitter = "Ticket Submitter";
        /// <summary>
        /// Ticket Handler can view tickets, edit them, change their type, add tasks and complete them.
        /// </summary>
        public static readonly string TicketHandler = "Ticket Handler";
        /// <summary>
        /// Ticket Reviewer can review other user's tickets, but can't make changes.
        /// </summary>
        public static readonly string TicketReviewer = "Ticket Reviewer";
        /// <summary>
        /// Asset Admin can create assets and delete them.
        /// </summary>
        public static readonly string AssetAdmin = "Asset Admin";
        /// <summary>
        /// Asset Manager can edit assets.
        /// </summary>
        public static readonly string AssetManager = "Asset Manager";
        /// <summary>
        /// Asset Assigner can assign/remove assets to/from users
        /// </summary>
        public static readonly string AssetAssigner = "Asset Assigner";
        /// <summary>
        /// Grants readonly access to assets to view properties and assignments.
        /// </summary>
        public static readonly string AssetReviewer = "Asset Reviewer";
        /// <summary>
        /// Allows downloading and deleting all files using the File Manager.
        /// </summary>
        public static readonly string FileAdmin = "File Admin";
        /// <summary>
        /// Allows downloading and deleting all files owned by the user.
        /// </summary>
        public static readonly string FileOwnEditor = "File Own Editor";


    }





}
