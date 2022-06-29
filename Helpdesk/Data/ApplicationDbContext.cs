using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Helpdesk.Data;

namespace Helpdesk.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        /// <summary>
        /// Site-wide Configuration Options
        /// </summary>
        public DbSet<ConfigOpt> ConfigOpts { get; set; }
        /// <summary>
        /// Users in the system
        /// </summary>
        public DbSet<HelpdeskUser> HelpdeskUsers { get; set; }
        /// <summary>
        /// Roles that can be assigned to users. Roles grant users permissions to do things.
        /// </summary>
        public DbSet<HelpdeskRole> HelpdeskRoles { get; set; }
        /// <summary>
        /// Claims control access to actions and sections of the site. 
        /// Add claims to a role and assign a role to a user to grant the user permission to do things.
        /// </summary>
        public DbSet<HelpdeskClaim> HelpdeskClaims { get; set; }
        /// <summary>
        /// Logical organization for assets, tickets, users, etc.
        /// </summary>
        public DbSet<Group> Groups { get; set; }
        /// <summary>
        /// A supervisor will have one or more team members assigned to them
        /// When a TeamMember submits a ticket, their supervisor is automatically added as a watcher and is notified.
        /// </summary>
        public DbSet<TeamMember> TeamMembers { get; set; }

        /// <summary>
        /// Defines what responsibilities the supervisor has for the subordinate
        /// </summary>
        public DbSet<TeamMemberResponsibility> TeamMemberResponsibilities { get; set; }
        /// <summary>
        /// Lists responsiblities a supervisor (with one or more team members) has. Helps the support desk
        /// to validate requests from a supervisor so that they know if they can perform a requested action.
        /// </summary>
        public DbSet<SupervisorResponsibility> SupervisorResponsibilities { get; set; }
        /// <summary>
        /// Site navigation template. Hides and shows menu items on the site.
        /// </summary>
        public DbSet<SiteNavTemplate> SiteNavTemplates { get; set; }
        /// <summary>
        /// Defines licenses that can be assigned to users and assets.
        /// </summary>
        public DbSet<LicenseType> LicenseType { get; set; }
        /// <summary>
        /// Assigns a license to a user
        /// </summary>
        public DbSet<UserLicenseAssignment> UserLicenseAssignments { get; set; }
        /// <summary>
        /// Assigns a license to an asset
        /// </summary>
        public DbSet<AssetLicenseAssignment> AssetLicenseAssignments { get; set; }
        /// <summary>
        /// Stores uploaded files
        /// </summary>
        public DbSet<FileUpload> FileUploads { get; set; }
        /// <summary>
        /// Identifies what kind of document a FileUploads is.
        /// </summary>
        public DbSet<DocumentType> DocumentTypes { get; set; }
        /// <summary>
        /// Devices, etc. that are available to assign to people and select for tickets.
        /// </summary>
        public DbSet<Asset> Assets { get; set; }
        /// <summary>
        /// When a change is made to an assset, a record is created.
        /// </summary>
        public DbSet<AssetChangeLog> AssetChangeLogs { get; set; }
        /// <summary>
        /// Locations that an asset can be assigned to
        /// </summary>
        public DbSet<Location> Locations { get; set; }
        /// <summary>
        /// Status of an asset, such as ordered, deployed, etc.
        /// </summary>
        public DbSet<AssetStatus> AssetStatuses { get; set; }
        /// <summary>
        /// Manufacturer, used to filter for Asset Models
        /// </summary>
        public DbSet<Manufacturer> Manufacturers { get; set; }
        /// <summary>
        /// Type of asset, used to filter Asset Models
        /// </summary>
        public DbSet<AssetType> AssetTypes { get; set; }
        /// <summary>
        /// Model number of asset to identiy the specific kind of device
        /// </summary>
        public DbSet<AssetModel> AssetModels { get; set; }
        /// <summary>
        /// Vendors that things can be ordered from
        /// </summary>
        public DbSet<Vendor> Vendors { get; set; }
        /// <summary>
        /// Status that a ticket can be in, such as new, in progress, rejected, closed, etc.
        /// </summary>
        public DbSet<TicketStatus> TicketStatuses { get; set; }
        /// <summary>
        /// Type of tickets that can exist.
        /// </summary>
        public DbSet<TicketType> TicketTypes { get; set; }
        /// <summary>
        /// People who are watching a ticket, can get updates, post comments, etc.
        /// </summary>
        public DbSet<TicketWatcher> TicketWatchers { get; set; }
        /// <summary>
        /// Master record for a ticket, tracks submitter, handler, etc.
        /// </summary>
        public DbSet<TicketMaster> TicketMasters { get; set; }
        /// <summary>
        /// Priority of a ticket
        /// </summary>
        public DbSet<TicketPriority> TicketPriority { get; set; }
        /// <summary>
        /// Status of an task, such as new, complete, rejected, etc.
        /// </summary>
        public DbSet<TaskStatus> TaskStatuses { get; set; }
        /// <summary>
        /// Defines the kind of tasks that can exist
        /// </summary>
        public DbSet<TaskType> TaskTypes { get; set; }
        /// <summary>
        /// Tasks the are required to be completed to complete a ticket.
        /// </summary>
        public DbSet<TicketTask> TicketTasks { get; set; }
        /// <summary>
        /// Defines the default tasks that are added to a ticket when it is created or has the type changed.
        /// </summary>
        public DbSet<TicketTypeDefaultTask> TicketTypeDefaultTasks { get; set; }
        /// <summary>
        /// Specifies the fields that are added to a ticket for a specified TicketType
        /// </summary>
        public DbSet<TicketTypeDefaultField> TicketTypeDefaultFields { get; set; }


        /*
        TO DO: 
-- Ticket Definition Tables
TicketType - Defines the types of tickets that can exist
TicketTypeDefaultTask - Defines the tasks that will be added to a ticket when it is created (or type changed)
TicketTypeDefaultField - Defines the fields that are added to a ticket when it is create (or type changed)
TaskType - Defines the kind of tasks that can be added to a ticket.
TaskTypeField - Defines the kind of fields that are added to a task
FieldType - Defines the kind of data that a field will store (data type), or what it will reference


-- Ticket Instance Tables
TicketMaster - Instances of Tickets
TicketField - Fields that are part of a ticket
TicketTask - Tasks that are a assigned to a ticket
TicketTaskField - Fields that are part of a task
FieldData<type> - Stores data for TicketField and TicketTaskField records
 



         */
    }
}