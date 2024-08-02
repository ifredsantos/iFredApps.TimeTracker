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
         public static Task<List<TimeManagerTaskSession>> GetAllSessions(int user_id)
         {
            //TODO: Get config by singleton
            return await new WebApiClient(appConfig.webapi_connection_config.baseaddress).GetAsync<List<TimeManagerTaskSession>>("Session?user_id={0}", user_id);
         }
      }
   }
}
