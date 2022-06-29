using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helpdesk.Data
{
    public class FieldDataNumber
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string EntityId { get; set; }
        [Required]
        public int FieldTypeId { get; set; }
        [Required]
        [Column(TypeName = "decimal(28,8)")]
        public decimal Value { get; set; }
    }
}
