using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Data
{
    public class UserLicenseAssignment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public HelpdeskUser HelpdeskUser { get; set; }

        [Required]
        public LicenseType LicenseType { get; set; }

        public string ProductCode { get; set; } = string.Empty;

    }
}
