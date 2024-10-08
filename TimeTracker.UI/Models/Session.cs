using System;

namespace TimeTracker.UI.Models
{
   public class Session
   {
      public int session_id { get; set; }
      public int user_id { get; set; }
      public DateTime start_date { get; set; }
      public DateTime? end_date { get; set; }
      public string? description { get; set; }
      public string? observation { get; set; }
   }
}
