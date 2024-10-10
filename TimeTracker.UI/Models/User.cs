using System;

namespace TimeTracker.UI.Models
{
   public class User
   {
      public int user_id { get; set; }
      public string username { get; set; }
      public string name { get; set; }
      public string email { get; set; }
      public string password { get; set; }
      public DateTime created_at { get; set; }
   }
}
