using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Skipperu.Data.Requests;
using Skipperu.Data.Users.data;
using Skipperu.Data.Users.data.ExternalAuth.data;

namespace Skipperu.Data
{
    public class UserAuthenticationDBcontext : IdentityDbContext<IdentityUser>
    {
        public UserAuthenticationDBcontext(DbContextOptions options) : base(options)
        {
        }

        protected UserAuthenticationDBcontext()
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //This Relationship Causes Issues when Creating Root Folders
            builder.Entity<GlobalUser>().HasOne(g => g.AspUser).WithMany().HasForeignKey(g => g.AspFK);
            builder.Entity<GlobalUser>().HasOne(g => g.ExternalAuthUser).WithMany().HasForeignKey(g => g.ExternalAuthFK);
            builder.Entity<GlobalUser>().HasKey(g => g.GlobalUserID);
            builder.Entity<ExternalAuthUser>().HasKey(g => g.PrimaryKey);
            builder.Entity<RequestDBstore>().HasKey(g => g.RequestID);
            builder.Entity<Collection>().HasKey(g => g.FolderRootID);
            builder.Entity<Collection>().HasOne(g => g.UserNav).WithMany().HasForeignKey(x => x.GlobalUserID);
            builder.Entity<GlobalUser>().HasIndex(x => x.UserName).IsUnique();

            builder.Entity<Collection>()
                .HasMany(property => property.SavedRequests)
                .WithOne()
                .HasForeignKey(x => x.ParentFolderID);

            base.OnModelCreating(builder);
        }

        public DbSet<GlobalUser> GlobalUsers { get; set; }
        public DbSet<ExternalAuthUser> ExternalAuthUsers { get; set; }
        public DbSet<RequestDBstore> requests { get; set;  }
        public DbSet<Collection> GlobalRequestCollection { get; set; }
    }
}
