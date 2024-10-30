using Microsoft.EntityFrameworkCore;
using iFredApps.TimeTracker.Core.Models;

namespace iFredApps.TimeTracker.Data.Data
{
   public class AppDbContext : DbContext
   {
      public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

      public DbSet<User> Users { get; set; }
      public DbSet<Session> Sessions { get; set; }

      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
         modelBuilder.Entity<User>().ToTable("users").HasKey(t => t.user_id);
         modelBuilder.Entity<Session>().ToTable("sessions").HasKey(t => t.session_id);

         base.OnModelCreating(modelBuilder);
      }
   }
}
