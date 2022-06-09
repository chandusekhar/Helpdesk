using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Data
{
    public class AssetModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public Manufacturer Manufacturer { get; set; }
        [Required]
        public AssetType AssetType { get; set; }
    }
}
