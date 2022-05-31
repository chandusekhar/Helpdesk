using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Data
{
    /// <summary>
    /// Specifies a collection of permissions that a user has that will let them do things
    /// on the site.
    /// </summary>
    public class HelpdeskRole
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }
        
        /// <summary>
        /// Users that are assigned this role.
        /// </summary>
        public ICollection<HelpdeskUser> Users { get; set; }

        /// <summary>
        /// Claims that this role grants access to.
        /// </summary>
        public ICollection<HelpdeskClaim> Claims { get; set; }
    }
}
