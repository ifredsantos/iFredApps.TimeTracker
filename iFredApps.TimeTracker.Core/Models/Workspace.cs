using System.ComponentModel.DataAnnotations;

namespace iFredApps.TimeTracker.Core.Models
{
   public class Workspace
   {
      public int? workspace_id { get; set; }
      [Required]
      [StringLength(60)]
      public string? name { get; set; }
      [Required]
      public int user_id { get; set; }
      [Required]
      public bool is_default { get; set; }
      public DateTime created_at { get; set; }
   }
}
