using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Data
{
    public class TicketWatcher
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public HelpdeskUser User { get; set; }
        [Required]
        [Display(Name = "Email Follow-up")]
        public bool EmailFollowUp { get; set; }
    }
}
