using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTracker.SL;

namespace TimeTracker.DL
{
   public class SessionContext : DbContext
   {
      public DbSet<sSession> sessions { get; set; }

      public SessionContext(DbContextOptions<SessionContext> options) : base(options)
      {

      }

      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
         modelBuilder.Entity<sSession>()
            .ToTable("sessions")
            .HasKey(s => s.session_id);

         modelBuilder.Entity<sSession>().Property(s => s.session_id).ValueGeneratedOnAdd().IsRequired();
         modelBuilder.Entity<sSession>().Property(s => s.user_id).IsRequired();
         modelBuilder.Entity<sSession>().Property(s => s.workspace_id).IsRequired();
         modelBuilder.Entity<sSession>().Property(s => s.start_date).IsRequired();
         modelBuilder.Entity<sSession>().Property(s => s.description).HasMaxLength(255).IsRequired();
         modelBuilder.Entity<sSession>().Property(s => s.observation).HasMaxLength(500);
      }
   }
}
