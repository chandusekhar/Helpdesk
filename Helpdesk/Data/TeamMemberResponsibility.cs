using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Data
{
    public class TeamMemberResponsibility
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public TeamMember TeamMember { get; set; }
        [Required]
        public SupervisorResponsibility Responsibility { get; set; }
    }
}
