using System.ComponentModel.DataAnnotations;

namespace iFredApps.TimeTracker.Core.Models
{
   public class UserBase
   {
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

   public class User : UserBase
   {
      public int user_id { get; set; }

      // Password reset fields
      public string? password_reset_token { get; set; }
      public DateTime? password_reset_expires_at { get; set; }
   }

   public class UserSignUp : UserBase
   {
      public string? confirm_password { get; set; }
   }

   public class UserLoginResponse : User
   {
      public string token { get; set; }
   }
}
