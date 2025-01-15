using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using iFredApps.TimeTracker.UI.Utils;
using iFredApps.Lib.Wpf.Execption;
using iFredApps.Lib;

namespace iFredApps.TimeTracker.UI.Models
{
   public static class DatabaseManager
   {
      public static async Task<TimeManagerDatabaseData> LoadData(int workspace_id)
      {
         TimeManagerDatabaseData result = new TimeManagerDatabaseData();
         try
         {
            AppConfig appConfig = SettingsLoader<AppConfig>.Instance.Data;

            if (appConfig != null)
            {
               if (appConfig.webapi_connection_config == null)
                  throw new Exception("It is necessary to parameterize the webapi configuration!");

               var sessionsResult = await WebApiCall.Sessions.GetAllSessions(AppWebClient.Instance.GetClient(), AppWebClient.Instance.GetLoggedUserData().user_id, workspace_id);

               var sessions = sessionsResult.TrataResposta();
               if (!sessions.IsNullOrEmpty())
               {
                  DateTime minDateDisplay = DateTime.Now.AddDays(-30);
                  sessions.RemoveAll(x => x.start_date < minDateDisplay);
               }

               result = new TimeManagerDatabaseData
               {
                  sessions = sessions,
                  uncompleted_session = sessions?.Find(x => x.end_date == null)
               };
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }

         return result;
      }

      public static async Task<TimeManagerTaskSession> CreateSession(TimeManagerTaskSession session, EventHandler<NotificationEventArgs> OnNotificationShow)
      {
         TimeManagerTaskSession result = null;
         try
         {
            AppConfig appConfig = SettingsLoader<AppConfig>.Instance.Data;
            if (appConfig != null)
            {
               if (appConfig.webapi_connection_config == null)
                  throw new Exception("It is necessary to parameterize the webapi configuration!");

               session.user_id = AppWebClient.Instance.GetLoggedUserData().user_id;

               var createResult = await WebApiCall.Sessions.CreateSession(AppWebClient.Instance.GetClient(), session);
               result = createResult?.TrataResposta();
               if (createResult != null && createResult.Success)
               {
                  OnNotificationShow?.Invoke(null, new NotificationEventArgs("Data synchronized successfully!", 3));
               }
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }

         return result;
      }

      public static async Task<TimeManagerTaskSession> UpdateSession(TimeManagerTaskSession session, EventHandler<NotificationEventArgs> OnNotificationShow)
      {
         TimeManagerTaskSession result = null;
         try
         {
            AppConfig appConfig = SettingsLoader<AppConfig>.Instance.Data;
            if (appConfig != null)
            {
               if (appConfig.webapi_connection_config == null)
                  throw new Exception("It is necessary to parameterize the webapi configuration!");

               session.user_id = AppWebClient.Instance.GetLoggedUserData().user_id;

               var updateResult = await WebApiCall.Sessions.UpdateSession(AppWebClient.Instance.GetClient(), session);
               result = updateResult?.TrataResposta();

               if (updateResult != null && updateResult.Success)
               {
                  OnNotificationShow?.Invoke(null, new NotificationEventArgs("Data synchronized successfully!", 3));
               }
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }

         return result;
      }

      public static async Task DeleteSession(int sessionID, EventHandler<NotificationEventArgs> OnNotificationShow)
      {
         try
         {
            AppConfig appConfig = SettingsLoader<AppConfig>.Instance.Data;
            if (appConfig != null)
            {
               if (appConfig.webapi_connection_config == null)
                  throw new Exception("It is necessary to parameterize the webapi configuration!");

               await WebApiCall.Sessions.DeleteSession(AppWebClient.Instance.GetClient(), sessionID);

               OnNotificationShow?.Invoke(null, new NotificationEventArgs("Data synchronized successfully!", 3));
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }
   }
}
