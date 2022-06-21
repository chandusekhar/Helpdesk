namespace Helpdesk.Infrastructure
{
    public static class TaskStatusStrings
    {
        /// <summary>
        /// New Task
        /// </summary>
        public static string NewTask = "New";
        /// <summary>
        /// Assigned to an agent
        /// </summary>
        public static string OpenTask = "Open";
        /// <summary>
        /// Waiting for response from submitter
        /// </summary>
        public static string OnHoldTask = "On-Hold";
        /// <summary>
        /// Waiting for internal response
        /// </summary>
        public static string PendingTask = "Pending";
        /// <summary>
        /// Task is completed
        /// </summary>
        public static string CompleteTask = "Complete";
        /// <summary>
        /// Task will not be completed
        /// </summary>
        public static string RejectedTask = "Rejected";
        /// <summary>
        /// Nothing to do, task ignored
        /// </summary>
        public static string IgnoredTask = "Ignored";
    }
}
