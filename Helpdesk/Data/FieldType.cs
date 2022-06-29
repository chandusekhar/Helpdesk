using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Data
{
    public class FieldType
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public bool AllowNulls { get; set; }
        [Required]
        public FieldDataTypes DataType { get; set; }
        [Required]
        public ReferenceTargetTypes ReferenceTargetType { get; set; }
        /// <summary>
        /// For FieldDataTypes>EntityFieldEditor only
        /// Name of the field that defines the ID of the entity that this field reference should point to.
        /// This must be a field defined on the entity that this field is attached to.
        /// </summary>
        public string? ReferenceDefinitionField { get; set; }
    }
}
