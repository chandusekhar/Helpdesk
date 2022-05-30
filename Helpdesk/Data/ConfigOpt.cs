using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Data
{
    public class ConfigOpt
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Category { get; set; } = String.Empty;

        [Required]
        public string Key { get; set; } = String.Empty;

        public string Value { get; set; } = String.Empty;

        public int? Order { get; set; }
    }
}
