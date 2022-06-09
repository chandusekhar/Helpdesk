using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Data
{
    public class AssetStatus
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
