using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Data
{
    /// <summary>
    /// Defines the kind of fields that are added to a task
    /// </summary>
    public class TaskTypeField
    {
        [Key]
        public int Id { get; set; }
    }
}
