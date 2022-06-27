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
        /// Shows the Ticket link in the Navbar. This takes the user to the My Tickets screen.
        /// </summary>
        [Required] 
        public bool TicketLink { get; set; }

        /// <summary>
        /// Shows the ticket menu that allows searching for tickets, showing all active, etc.
        /// Meant for Ticket Admins
        /// </summary>
        public bool TicketMenu { get; set; }

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
        /// Show Supervisor Responsibilities link in the Configuration menu
        /// </summary>
        public bool SupRespsLink { get; set; }
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
        /// Shows the TaskStatuses link on the Configuration menu
        /// </summary>
        [Required]
        public bool TaskStatusesLink { get; set; }

        /// <summary>
        /// Shows the TicketPriorities link on the Configuration menu
        /// </summary>
        [Required]
        public bool TicketPrioritiesLink { get; set; }

        /// <summary>
        /// Shows the TicketStatuses link on the Configuration menu
        /// </summary>
        [Required]
        public bool TicketStatusesLink { get; set; }

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
