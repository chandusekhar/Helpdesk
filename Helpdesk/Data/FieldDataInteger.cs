using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Data
{
    public class FieldDataInteger
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string EntityId { get; set; }
        [Required]
        public int FieldTypeId { get; set; }
        [Required]
        public int Value { get; set; }
    }
}
