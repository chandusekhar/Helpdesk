using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Data
{
    public class TicketTask
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public TicketMaster TicketMaster { get; set; }
        [Required]
        public TaskStatus TaskStatus { get; set; }
    }
}
