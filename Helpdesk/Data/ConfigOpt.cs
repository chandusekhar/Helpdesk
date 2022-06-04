using Helpdesk.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Data
{
    public class ConfigOpt
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Category { get; set; } = string.Empty;

        [Required]
        public string Key { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;

        public int? Order { get; set; }
        [Required]
        public ReferenceTypes ReferenceType { get; set; }
    }
}
