using System.ComponentModel.DataAnnotations;

namespace iFredApps.TimeTracker.Core.Models
{
   public class User
   {
      public int user_id { get; set; }
      [Required]
      [StringLength(60)]
      public string? username { get; set; }
      [Required]
      [StringLength(120)]
      public string? name { get; set; }
      [Required]
      [StringLength(150)]
      public string? email { get; set; }
      [Required]
      [StringLength(100)]
      public string? password { get; set; }
      public DateTime created_at { get; set; }
   }

   public class UserLoginResponse : User
   {
      public string token { get; set; }
   }
}
