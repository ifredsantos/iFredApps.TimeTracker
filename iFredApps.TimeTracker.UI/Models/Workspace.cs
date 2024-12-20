using System;
using System.ComponentModel.DataAnnotations;

namespace iFredApps.TimeTracker.UI.Models
{
   public class Workspace
   {
      public int workspace_id { get; set; }
      public string? name { get; set; }
      public int user_id { get; set; }
      public bool is_default { get; set; }
      public DateTime created_at { get; set; }
   }
}
