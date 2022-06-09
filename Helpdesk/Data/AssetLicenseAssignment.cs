using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Data
{
    public class AssetLicenseAssignment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Asset Asset { get; set; }

        [Required]
        public LicenseType LicenseType { get; set; }

        public string ProductCode { get; set; } = string.Empty;
    }
}
