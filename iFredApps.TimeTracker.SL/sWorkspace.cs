using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFredApps.TimeTracker.SL
{
   public class sWorkspace
   {
      public int? workspace_id { get; set; }
      public string? name { get; set; }
      public int user_id { get; set; }
      public bool is_default { get; set; }
      public DateTime created_at { get; set; }
   }
}
