using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Data
{
    public class TicketType
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public bool IsSystemType { get; set; }

        /// <summary>
        /// HelpdeskClaim that must be held for a user to create a ticket of this type
        /// </summary>
        public string? CreationClaim { get; set; }
        /// <summary>
        /// HelpdeskClaim that must be held for a user to edit a ticket of this type
        /// </summary>
        public string? EditClaim { get; set; }
        /// <summary>
        /// HelpdeskClaim that must be held for a user to see the ticket.
        /// Without the claim, the user can't search for or view the claim at all.
        /// Make sure you add this claim if you are going to grant CreationClaim or EditClaim
        /// </summary>
        public string? ViewClaim { get; set; }

        public ICollection<TicketActionType> DefaultActions { get; set; }
    }
}
