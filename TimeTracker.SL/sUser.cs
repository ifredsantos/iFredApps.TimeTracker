using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTracker.SL
{
   public class sUser : IdentityUser
   {
      public string? username { get; set; }
      public string? name { get; set; }
      public string? email { get; set; }
      public string? password { get; set; }
   }
}
