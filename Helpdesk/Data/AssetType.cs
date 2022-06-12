using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Data
{
    public class AssetType
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
