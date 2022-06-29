using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Data
{
    public class FieldDataBoolean
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string EntityId { get; set; }
        [Required]
        public int FieldTypeId { get; set; }
        [Required]
        public bool Value { get; set; }
    }
}
