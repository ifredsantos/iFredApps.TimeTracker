using iFredApps.Lib.WebApi;
using iFredApps.TimeTracker.SL;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimeTracker.SL;

namespace iFredApps.TimeTracker.UI.Models
{
   public static class WebApiCall
   {
      public static class Users
      {
         public static async Task<ApiResponse<sUser>> Login(WebApiClient webClient, string user, string password)
         {
            return await webClient.PostAsync<ApiResponse<sUser>>("Users/Login", new LoginModel { UserSearchTerm = user, Password = password });
         }

         public static async Task<ApiResponse<sUser>> SignUp(WebApiClient webClient, sUserSignUp signUpData)
         {
            return await webClient.PostAsync<ApiResponse<sUser>>("Users/SignUp", signUpData);
         }
      }

      public static class Sessions
      {
         public static async Task<ApiResponse<List<TimeManagerTaskSession>>> GetSessions(WebApiClient webClient, int user_id, int workspace_id, DateTime? start_date = null, DateTime? end_date = null)
         {
            return await webClient.GetAsync<ApiResponse<List<TimeManagerTaskSession>>>("Session/GetSessions/{0}/{1}/{2}/{3}", user_id, workspace_id, start_date?.ToString("yyyy-MM-dd"), end_date.HasValue ? end_date.Value.ToString("yyyy-MM-dd") : null);
         }

         public static async Task<ApiResponse<List<TimeManagerTaskSession>>> GetSessionsByDescription(WebApiClient webClient, GetSessionsRequest request)
         {
            return await webClient.PostAsync<ApiResponse<List<TimeManagerTaskSession>>>("Session/GetSessionsByDescription", request);
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
         public static async Task<ApiResponse<List<sWorkspace>>> GetAllByUserId(WebApiClient webClient, int user_id)
         {
            return await webClient.GetAsync<ApiResponse<List<sWorkspace>>>("Workspace/GetAllByUserId/{0}", user_id);
         }

         public static async Task<ApiResponse<sWorkspace>> Create(WebApiClient webClient, sWorkspace workspace)
         {
            return await webClient.PostAsync<ApiResponse<sWorkspace>>("Workspace/Create", workspace);
         }

         public static async Task<ApiResponse<sWorkspace>> Update(WebApiClient webClient, sWorkspace workspace)
         {
            return await webClient.PutAsync<ApiResponse<sWorkspace>>("Workspace/Update/{0}", workspace, workspace.workspace_id);
         }

         public static async Task<ApiResponse<bool>> Delete(WebApiClient webClient, int workspaceID)
         {
            return await webClient.DeleteAsync<ApiResponse<bool>>("Workspace/Delete/{0}", workspaceID);
         }
      }
   }
}
