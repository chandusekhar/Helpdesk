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
        public string Supervisor { get; set; }

        [Required]
        public string Subordinate { get; set; }
        
        public ICollection<SupervisorResponsibility> SupervisorResponsibilities { get; set; }

    }
}
