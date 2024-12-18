using iFredApps.Lib.WebApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace iFredApps.TimeTracker.UI.Models
{
   public static class WebApiCall
   {
      public static class Sessions
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

      public static class Workspaces
      {
         public static async Task<List<Workspace>> GetAllByUserId(WebApiClient webClient, int user_id)
         {
            return await webClient.GetAsync<List<Workspace>>("Workspace/GetAllByUserId/{0}", user_id);
         }

         public static async Task<Workspace> Create(WebApiClient webClient, Workspace workspace)
         {
            return await webClient.PostAsync<Workspace>("Workspace/Create", workspace);
         }

         public static async Task<Workspace> Update(WebApiClient webClient, Workspace workspace)
         {
            return await webClient.PutAsync<Workspace>("Workspace/Update/{0}", workspace, workspace.workspace_id);
         }

         public static async Task Delete(WebApiClient webClient, int workspaceID)
         {
            await webClient.DeleteAsync<DBNull>("Workspace/Delete/{0}", workspaceID);
         }
      }
   }
}
