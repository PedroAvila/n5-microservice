using Microsoft.EntityFrameworkCore;
using N5.Api.Models;

namespace N5.Api.Data
{
    public class AccessManagementContext: DbContext
    {
        public AccessManagementContext(DbContextOptions<AccessManagementContext> options) : base(options) { }

        public DbSet<Permission> Permissions { get; set; }
        public DbSet<PermissionType> PermissionTypes { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AccessManagementContext).Assembly);

            modelBuilder.Entity<PermissionType>().HasData(
                new PermissionType { Id = 1, Description = "Administrator" },
                new PermissionType { Id = 2, Description = "Manager" },
                new PermissionType { Id = 3, Description = "Supervisor" }
             );

            base.OnModelCreating(modelBuilder);
        }
    }
}
