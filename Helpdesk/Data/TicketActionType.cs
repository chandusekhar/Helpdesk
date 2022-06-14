using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Data
{
    public class TicketActionType
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public bool IsSystemType { get; set; }

        /// <summary>
        /// HelpdeskClaim that must be held for a user to add an action of this type to a ticket
        /// </summary>
        public string? CreationClaim { get; set; }
        /// <summary>
        /// HelpdeskClaim that must be held for a user to edit an action of this type on a ticket
        /// </summary>
        public string? EditClaim { get; set; }
        /// <summary>
        /// HelpdeskClaim that must be held for a user to see this action type
        /// Without the claim, the user can't search for or view the action type at all.
        /// Make sure you add this claim if you are going to grant CreationClaim or EditClaim
        /// </summary>
        public string? ViewClaim { get; set; }
    }
}
