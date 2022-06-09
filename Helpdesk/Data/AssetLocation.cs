using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Data
{
    public class AssetLocation
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

    }
}
