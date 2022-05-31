using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Data
{
    /// <summary>
    /// Defines a master record for a ticket.
    /// </summary>
    public class TicketMaster
    {
        [Key]
        public string Id { get; set; }
        /// <summary>
        /// Title
        /// </summary>
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        /// <summary>
        /// User that created this ticket.
        /// </summary>
        [Required]
        
        public string Creator { get; set; }
    }
}
