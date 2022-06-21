namespace Helpdesk.Infrastructure
{
    public class TicketStatusStrings
    {
        /// <summary>
        /// New Ticket
        /// </summary>
        public static string NewTicket = "New";
        /// <summary>
        /// Assigned to an agent
        /// </summary>
        public static string OpenTicket = "Open";
        /// <summary>
        /// Waiting for response from submitter
        /// </summary>
        public static string OnHoldTicket = "On-Hold";
        /// <summary>
        /// Waiting for internal response
        /// </summary>
        public static string PendingTicket = "Pending";
        /// <summary>
        /// Ticket is completed
        /// </summary>
        public static string CompleteTicket = "Complete";
        /// <summary>
        /// Ticket will not be completed
        /// </summary>
        public static string RejectedTicket = "Rejected";
        /// <summary>
        /// Ticket has been closed
        /// </summary>
        public static string ClosedTicket = "Closed";
    }
}
