using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SMART.Web.Models.AI;
using SMART.Web.Models.HRM;
using SMART.Web.Models.INV;
using SMART.Web.Models.MST;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;


namespace SMART.Web.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        #region TABLES
        public DbSet<Company> Companies { get; set; }
        public DbSet<Department> Departments { get; set; }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeComplain> EmployeeComplains { get; set; }

        public DbSet<ItemUnit> ItemUnits { get; set; }
        public DbSet<AiChatSession> AiChatSessions { get; set; }
        public DbSet<AiChatMessage> AiChatMessages { get; set; }
        public DbSet<AiAgentSetting> AiAgentSettings { get; set; }
        #endregion

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AiChatMessage>()
                .HasRequired(x => x.Session)
                .WithMany(x => x.Messages)
                .HasForeignKey(x => x.SessionId)
                .WillCascadeOnDelete(false);
        }
    }

}