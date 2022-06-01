using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Data
{
    public class HelpdeskUser
    {
        public HelpdeskUser()
        {

        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string IdentityUserId { get; set; }
        [Required]
        public string GivenName { get; set; }
        [Required]
        public string Surname { get; set; }
        
        public string? JobTitle { get; set; }

        public string? Company { get; set; }

        public SiteNavTemplate SiteNavTemplate { get; set; }
        /// <summary>
        /// If user is a supervisor, this is a list of employees they manage, and what their responsibilities are.
        /// </summary>
        public ICollection<TeamMember> TeamMembers { get; set; }

        /// <summary>
        /// Roles this user has.
        /// </summary>
        public ICollection<HelpdeskRole> Roles { get; set; }
    }
}
