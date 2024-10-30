using System.ComponentModel.DataAnnotations;

namespace iFredApps.TimeTracker.Core.Models
{
   public class Session
   {
      public int session_id { get; set; }
      public int user_id { get; set; }
      public DateTime start_date { get; set; }
      public DateTime? end_date { get; set; }
      [Required]
      [StringLength(255)]
      public string? description { get; set; }
      [StringLength(500)]
      public string? observation { get; set; }
   }
}
