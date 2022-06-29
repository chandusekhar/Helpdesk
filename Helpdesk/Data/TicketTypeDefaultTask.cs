using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Data
{
    /// <summary>
    /// Defines the default tasks that are added to a ticket when it is created or has the type changed.
    /// </summary>
    public class TicketTypeDefaultTask
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// Sort on this field and create the tasks in order.
        /// </summary>
        public int CreateOrder { get; set; }
        /// <summary>
        /// What TicketType is this specifying a task for
        /// </summary>
        public TicketType TicketType { get; set; }
        /// <summary>
        /// Task to specify for the TicketType
        /// </summary>
        public TaskType TaskType { get; set; }
    }
}
