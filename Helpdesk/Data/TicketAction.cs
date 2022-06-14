using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Data
{
    public class TicketAction
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public TicketMaster TicketMaster { get; set; }
        [Required]
        public ActionStatus ActionStatus { get; set; }
    }
}
