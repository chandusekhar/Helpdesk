using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Data
{
    /// <summary>
    /// Instance of a task that must be completed before a ticket can be marked as complete.
    /// </summary>
    public class TicketTask
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public TicketMaster TicketMaster { get; set; }
        [Required]
        public TaskStatus TaskStatus { get; set; }
        [Required]
        public TaskType TaskType { get; set; }
    }
}
