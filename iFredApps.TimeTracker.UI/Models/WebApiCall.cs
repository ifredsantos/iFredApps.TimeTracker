using iFredApps.Lib.WebApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace iFredApps.TimeTracker.UI.Models
{
   public static class WebApiCall
   {
      public static class Session
      {
         public static async Task<List<TimeManagerTaskSession>> GetAllSessions(WebApiClient webClient, int user_id)
         {
            return await webClient.GetAsync<List<TimeManagerTaskSession>>("Session/GetSessions/{0}", user_id);
         }

         public static async Task<TimeManagerTaskSession> CreateSession(WebApiClient webClient, TimeManagerTaskSession session)
         {
            return await webClient.PostAsync<TimeManagerTaskSession>("Session/CreateSession", session);
         }

         public static async Task<TimeManagerTaskSession> UpdateSession(WebApiClient webClient, TimeManagerTaskSession session)
         {
            return await webClient.PutAsync<TimeManagerTaskSession>("Session/UpdateSession/{0}", session, session.session_id);
         }

         public static async Task DeleteSession(WebApiClient webClient, int sessionID)
         {
            await webClient.DeleteAsync<DBNull>("Session/DeleteSession/{0}", sessionID);
         }
      }
   }
}
