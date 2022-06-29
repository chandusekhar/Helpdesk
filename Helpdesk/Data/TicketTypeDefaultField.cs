using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Data
{
    /// <summary>
    /// Specifies the fields that are added to a ticket for a specified TicketType
    /// </summary>
    public class TicketTypeDefaultField
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// What TicketType will have this field defined.
        /// </summary>
        public TicketType TicketType { get; set; }
        /// <summary>
        /// Field type to create on this ticket
        /// </summary>
        public FieldType FieldType { get; set; }
    }
}
