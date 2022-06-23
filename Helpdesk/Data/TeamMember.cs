using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Data
{
    /// <summary>
    /// Defines a supervisor/subordinate relationship
    /// </summary>
    public class TeamMember
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public HelpdeskUser Supervisor { get; set; }

        [Required]
        public HelpdeskUser Subordinate { get; set; }
        
        public virtual ICollection<SupervisorResponsibility> SupervisorResponsibilities { get; set; }

    }
}
