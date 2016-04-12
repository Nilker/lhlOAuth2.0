using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MyOAuth2.Server.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>().Property(u => u.UserName).HasMaxLength(128);

            //Uncomment this to have Email length 128 too (not neccessary)
            //modelBuilder.Entity<ApplicationUser>().Property(u => u.Email).HasMaxLength(128);

            modelBuilder.Entity<IdentityRole>().Property(r => r.Name).HasMaxLength(128);
        }
    }
}