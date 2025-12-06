using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFredApps.TimeTracker.UI.Models
{
   public class ForgotPasswordRequest
   {
      public string email { get; set; }
   }

   public class ResetPasswordRequest
   {
      public string token { get; set; }
      public string newPassword { get; set; }
   }
}
