using iFredApps.Lib.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTracker.UI.Models
{
   public static class WebApiCall
   {
      public static class Session
      {
         public static async Task<List<TimeManagerTaskSession>> GetAllSessions(WebApiClient webClient, int user_id)
         {
            return await webClient.GetAsync<List<TimeManagerTaskSession>>("Session/GetSessions?user_id={0}", user_id);
         }
      }
   }
}
