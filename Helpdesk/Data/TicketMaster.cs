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
        /// The kind of ticket this is. A new ticket can be created without a ticket type
        /// </summary>
        public TicketType? TicketType { get; set; }
        /// <summary>
        /// Title of the ticket
        /// </summary>
        [Required]
        public string Title { get; set; }
        /// <summary>
        /// Submitter's description of the problem
        /// </summary>
        [Required]
        public string Description { get; set; }
        /// <summary>
        /// User that submitted the requests that this ticket be opened.
        /// Points to HelpdeskUser.
        /// </summary>
        [Required]
        public HelpdeskUser Requester { get; set; }
        /// <summary>
        /// List of followers to this ticket. Followers can see the status in the 
        /// site, and can optionally get email updates.
        /// </summary>
        public ICollection<TicketWatcher> TicketWatchers { get; set; }
        /// <summary>
        /// User who is assigned to handle this ticket.
        /// </summary>
        public HelpdeskUser? Handler { get; set; }
        /// <summary>
        /// Date Time when this ticket was created
        /// </summary>
        public DateTime CreationDate { get; set; }
        /// <summary>
        /// Deadline when this ticket should be completed by
        /// </summary>
        public DateTime? DueDate { get; set; }
        /// <summary>
        /// When was this ticket or any of the tasks in it last updated
        /// </summary
        public DateTime? LastUpdate { get; set; }
        /// <summary>
        /// What status is this ticket
        /// </summary>
        [Required]
        public TicketStatus TicketStatus { get; set; }
        /// <summary>
        /// Priority of the ticket
        /// </summary>
        [Required]
        public TicketPriority TicketPriority { get; set; }
        /// <summary>
        /// Tasks that have been added to this ticket
        /// </summary>
        public ICollection<TicketTask> Tasks { get; set; }

    }
}
