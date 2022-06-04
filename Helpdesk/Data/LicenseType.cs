using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Data
{
    public class LicenseType
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public int? Seats { get; set; }
        [Required]
        public bool IsDeviceLicense { get; set; }
        [Required] 
        public bool IsUserLicense { get; set; }
        [Required]
        public bool DeviceRequireProductCode { get; set; }
        [Required]
        public bool UserRequireProductCode { get; set; }
    }
}
