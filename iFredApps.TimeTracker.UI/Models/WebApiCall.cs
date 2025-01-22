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
         public static async Task<ApiResponse<List<TimeManagerTaskSession>>> GetSessions(WebApiClient webClient, int user_id, int workspace_id, DateTime? start_date = null, DateTime? end_date = null)
         {
            return await webClient.GetAsync<ApiResponse<List<TimeManagerTaskSession>>>("Session/GetSessions/{0}/{1}/{2}/{3}", user_id, workspace_id, start_date?.ToString("yyyy-MM-dd"), end_date.HasValue ? end_date.Value.ToString("yyyy-MM-dd") : null);
         }

         public static async Task<ApiResponse<TimeManagerTaskSession>> CreateSession(WebApiClient webClient, TimeManagerTaskSession session)
         {
            return await webClient.PostAsync<ApiResponse<TimeManagerTaskSession>>("Session/CreateSession", session);
         }

         public static async Task<ApiResponse<TimeManagerTaskSession>> UpdateSession(WebApiClient webClient, TimeManagerTaskSession session)
         {
            return await webClient.PutAsync<ApiResponse<TimeManagerTaskSession>>("Session/UpdateSession/{0}", session, session.session_id);
         }

         public static async Task<ApiResponse<bool>> DeleteSession(WebApiClient webClient, int sessionID)
         {
            return await webClient.DeleteAsync<ApiResponse<bool>>("Session/DeleteSession/{0}", sessionID);
         }
      }

      public static class Workspaces
      {
         public static async Task<ApiResponse<List<Workspace>>> GetAllByUserId(WebApiClient webClient, int user_id)
         {
            return await webClient.GetAsync<ApiResponse<List<Workspace>>>("Workspace/GetAllByUserId/{0}", user_id);
         }

         public static async Task<ApiResponse<Workspace>> Create(WebApiClient webClient, Workspace workspace)
         {
            return await webClient.PostAsync<ApiResponse<Workspace>>("Workspace/Create", workspace);
         }

         public static async Task<ApiResponse<Workspace>> Update(WebApiClient webClient, Workspace workspace)
         {
            return await webClient.PutAsync<ApiResponse<Workspace>>("Workspace/Update/{0}", workspace, workspace.workspace_id);
         }

         public static async Task<ApiResponse<bool>> Delete(WebApiClient webClient, int workspaceID)
         {
            return await webClient.DeleteAsync<ApiResponse<bool>>("Workspace/Delete/{0}", workspaceID);
         }
      }
   }
}
