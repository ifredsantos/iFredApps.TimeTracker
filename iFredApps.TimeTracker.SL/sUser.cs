using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTracker.SL
{
   public class sUser
   {
      public int user_id { get; set; }
      public string? username { get; set; }
      public string? name { get; set; }
      public string? email { get; set; }
      public string? password { get; set; }
      public DateTime created_at { get; set; }
   }
}
