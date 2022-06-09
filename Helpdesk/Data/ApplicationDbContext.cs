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

        public DbSet<ConfigOpt> ConfigOpts { get; set; }
        public DbSet<HelpdeskUser> HelpdeskUsers { get; set; }
        public DbSet<HelpdeskRole> HelpdeskRoles { get; set; }
        public DbSet<HelpdeskClaim> HelpdeskClaims { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<SupervisorResponsibility> SupervisorResponsibilities { get; set; }
        public DbSet<SiteNavTemplate> SiteNavTemplates { get; set; }
        public DbSet<LicenseType> LicenseType { get; set; }
        public DbSet<UserLicenseAssignment> UserLicenseAssignments { get; set; }
        public DbSet<AssetLicenseAssignment> AssetLicenseAssignments { get; set; }
        public DbSet<FileUpload> FileUploads { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<AssetChangeLog> AssetChangeLogs { get; set; }
        public DbSet<AssetLocation> AssetLocations { get; set; }
        public DbSet<AssetStatus> AssetStatuses { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<AssetType> AssetTypes { get; set; }
        public DbSet<AssetModel> AssetModels { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
    }
}