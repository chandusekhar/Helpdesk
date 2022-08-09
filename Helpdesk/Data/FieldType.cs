using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Data
{
    /// <summary>
    /// Defines the kind of data that a field will store (data type), or what it will reference
    /// </summary>
    public class FieldType
    {
        [Key]
        public int Id { get; set; }
    }
}
