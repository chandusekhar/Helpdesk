using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Data
{
    public class Manufacturer
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public ICollection<AssetModel> Models { get; set; }
    }
}
