using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Data
{
    public class SiteNavTemplate
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required] 
        public string Description { get; set; }
        /// <summary>
        /// Shows the Ticket link in the Navbar
        /// </summary>
        [Required] 
        public bool TicketLink { get; set; }
        /// <summary>
        /// Shows the Asset link in the Navbar
        /// </summary>
        [Required]
        public bool AssetLink { get; set; }
        /// <summary>
        /// Shows People link in the Navbar
        /// </summary>
        [Required] 
        public bool PeopleLink { get; set; }
        /// <summary>
        /// Show the Configuration Menu on the NavBar
        /// </summary>
        [Required]
        public bool ShowConfigurationMenu { get; set; }
        /// <summary>
        /// Show LicenseType link in the Configuration Menu
        /// </summary>
        [Required]
        public bool LicenseTypeLink { get; set; }
        /// <summary>
        /// Shows SiteSettings link in the Configuration Menu
        /// </summary>
        [Required]
        public bool SiteSettingsLink { get; set; }
        /// <summary>
        /// Shows the Groups link in the Configuration Menu
        /// </summary>
        [Required]
        public bool GroupsLink { get; set; }
        /// <summary>
        /// Shows the Import/Export link in the Configuration menu
        /// </summary>
        [Required]
        public bool ImportExportLink { get; set; }

        /// <summary>
        /// Shows the Asset Types link on the Configuration menu
        /// </summary>
        [Required]
        public bool AssetTypesLink { get; set; }

        /// <summary>
        /// Shows the Manufactuers link on the Configuration menu
        /// </summary>
        [Required]
        public bool ManufacturersLink { get; set; }

        /// <summary>
        /// Shows the Roles link on the Configuration menu
        /// </summary>
        [Required]
        public bool RoleAdminLink { get; set; }
        /// <summary>
        /// Shows the Locations link on the Configuration menu
        /// </summary>
        [Required]
        public bool LocationsLink { get; set; }

        /// <summary>
        /// Shows the Document Types link on the Configuration menu
        /// </summary>
        [Required]
        public bool DocumentTypesLink { get; set; }
        /// <summary>
        /// Show the file manager link on the nav bar menu
        /// </summary>
        [Required]
        public bool FileManagerLink { get; set; }
    }
}
