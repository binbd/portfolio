using Microsoft.EntityFrameworkCore;
using Profiolio_MVC.Models;

namespace Profiolio_MVC.Data
{
    public class myDbContext:DbContext
    {
        public myDbContext(DbContextOptions<myDbContext> options)
        : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        
        public DbSet<ViewerCounting> viewerCountings{get;set;}
        public DbSet<SettingsNumber> settingsNumbers{get;set;}


    }
}