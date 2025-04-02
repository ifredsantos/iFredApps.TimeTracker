using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFredApps.TimeTracker.UI.Utils
{
   public static class Utilities
   {
      public static DateTime GetDateTimeNow()
      {
         TimeZoneInfo gmtTimeZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
         return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, gmtTimeZone);
      }
   }
}
