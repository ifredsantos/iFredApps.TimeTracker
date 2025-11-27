using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFredApps.TimeTracker.UI.Models
{
   public class GetSessionsRequest
   {
      public int user_id { get; set; }
      public int workspace_id { get; set; }
      public string description { get; set; }
      public DateTime? start_date { get; set; }
      public DateTime? end_date { get; set; }
   }
}
