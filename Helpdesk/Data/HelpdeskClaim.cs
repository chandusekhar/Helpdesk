using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Data
{
    /// <summary>
    /// Defines a specific operation in the sytem.
    /// Assign claims to roles to create categories of users that have certain permissions
    /// </summary>
    public class HelpdeskClaim
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }

        [Required]
        public bool IsSystemType { get; set; }

        /// <summary>
        /// Roles that grant access to this claim
        /// </summary>
        public ICollection<HelpdeskRole> GrantingRoles { get; set; }
    }
}
