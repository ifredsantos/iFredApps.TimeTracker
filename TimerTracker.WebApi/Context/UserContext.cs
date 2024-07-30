using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTracker.SL;

namespace TimeTracker.WebApi.Context
{
   public class UserContext : IdentityDbContext<sUser>
   {
      public DbSet<sUser> users { get; set; }

      public UserContext(DbContextOptions<UserContext> options) : base(options)
      {

      }
   }
}
