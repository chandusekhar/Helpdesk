using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Data
{
    public class TicketStatus
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public bool IsSystemType { get; set; }
        [Required]
        public bool IsCompleted { get; set; }
        [Required]
        public bool Archived { get; set; }
        [Required]
        public int DisplayOrder { get; set; }
    }
}
