namespace Helpdesk.Data
{
    /// <summary>
    /// Defines a specific operation in the sytem.
    /// Assign claims to roles to create categories of users that have certain permissions
    /// </summary>
    public class HelpdeskClaim
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// Roles that grant access to this claim
        /// </summary>
        public ICollection<HelpdeskRole> GrantingRoles { get; set; }
    }
}
