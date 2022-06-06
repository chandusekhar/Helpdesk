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
    }
}