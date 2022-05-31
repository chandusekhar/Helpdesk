using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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
        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<SupervisorResponsibility> SupervisorResponsibilities { get; set; }


        
    }
}