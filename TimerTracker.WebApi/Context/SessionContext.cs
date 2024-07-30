using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTracker.SL;

namespace TimeTracker.WebApi.Context
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
            .HasKey(s => s.session_id);
      }
   }
}
