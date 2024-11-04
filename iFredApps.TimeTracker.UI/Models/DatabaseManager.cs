using Newtonsoft.Json;
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
      private static JSONDataBaseConfig defaultJsonConfig = new JSONDataBaseConfig
      {
         directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "iFredApps", "TimeTracker"),
         filename = !Debugger.IsAttached ? "db.json" : "db_test.json"
      };

      public static async Task<TimeManagerDatabaseData> LoadData()
      {
         TimeManagerDatabaseData result = new TimeManagerDatabaseData();
         try
         {
            AppConfig appConfig = SettingsLoader<AppConfig>.Instance.Data;

            if (appConfig != null)
            {
               if (appConfig.database_type == AppConfig.enDataBaseType.JSON)
               {
                  if (appConfig.json_database_config == null)
                     appConfig.json_database_config = defaultJsonConfig;

                  string databaseFileDir = Path.Combine(appConfig.json_database_config.directory, appConfig.json_database_config.filename);

                  if (File.Exists(databaseFileDir))
                  {
                     string tasksJSON = await File.ReadAllTextAsync(databaseFileDir);
                     var sessions = JsonConvert.DeserializeObject<List<TimeManagerTaskSession>>(tasksJSON);
                     result = new TimeManagerDatabaseData
                     {
                        sessions = sessions,
                        uncompleted_session = sessions?.Find(x => x.end_date == null)
                     };
                  }
               }
               else if (appConfig.database_type == AppConfig.enDataBaseType.WebApi)
               {
                  if (appConfig.webapi_connection_config == null)
                     throw new Exception("It is necessary to parameterize the webapi configuration!");

                  var sessions = await WebApiCall.Session.GetAllSessions(AppWebClient.Instance.GetClient(), AppWebClient.Instance.GetLoggedUserData().user_id);

                  if(!sessions.IsNullOrEmpty())
                  {
                     DateTime minDateDisplay = DateTime.Now.AddDays(-7);
                     sessions.RemoveAll(x => x.start_date < minDateDisplay);
                  }

                  result = new TimeManagerDatabaseData
                  {
                     sessions = sessions,
                     uncompleted_session = sessions?.Find(x => x.end_date == null)
                  };
               }
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }

         return result;
      }

      /// <summary>
      /// Supported for local database only
      /// </summary>
      /// <param name="sessions"></param>
      /// <param name="OnNotificationShow"></param>
      /// <returns></returns>
      private static Task SaveAllSessions(List<TimeManagerTaskSession> sessions, EventHandler<NotificationEventArgs> OnNotificationShow)
      {
         return Task.Factory.StartNew(() =>
         {
            try
            {
               AppConfig appConfig = SettingsLoader<AppConfig>.Instance.Data;
               if (appConfig != null)
               {
                  if (appConfig.database_type == AppConfig.enDataBaseType.JSON && appConfig.json_database_config != null)
                  {
                     string directory = appConfig.json_database_config.directory;
                     string filename = appConfig.json_database_config.filename;
                     string databaseFileDir = Path.Combine(directory, filename);

                     if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                     string tasksJSON = JsonConvert.SerializeObject(sessions);
                     File.WriteAllTextAsync(databaseFileDir, tasksJSON);
                  }
               }
            }
            catch (Exception ex)
            {
               ex.ShowException();
            }
         })
         .ContinueWith(t =>
         {
            OnNotificationShow?.Invoke(null, new NotificationEventArgs("Data synchronized successfully!", 3));
         }, TaskScheduler.FromCurrentSynchronizationContext());
      }

      public static async Task<TimeManagerTaskSession> CreateSession(TimeManagerTaskSession session, EventHandler<NotificationEventArgs> OnNotificationShow)
      {
         TimeManagerTaskSession result = null;
         try
         {
            AppConfig appConfig = SettingsLoader<AppConfig>.Instance.Data;
            if (appConfig != null)
            {
               if (appConfig.database_type == AppConfig.enDataBaseType.WebApi && appConfig.webapi_connection_config != null)
               {
                  session.user_id = AppWebClient.Instance.GetLoggedUserData().user_id;

                  result = await WebApiCall.Session.CreateSession(AppWebClient.Instance.GetClient(), session);

                  OnNotificationShow?.Invoke(null, new NotificationEventArgs("Data synchronized successfully!", 3));
               }
               else if (appConfig.database_type == AppConfig.enDataBaseType.JSON && appConfig.json_database_config != null)
               {
                  var storedData = await LoadData();

                  session.session_id = storedData.sessions.Count > 0 ? storedData.sessions.Max(x => x.session_id) + 1 : 1;

                  storedData.sessions.Add(session);

                  await SaveAllSessions(storedData.sessions, OnNotificationShow);
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
               if (appConfig.database_type == AppConfig.enDataBaseType.WebApi && appConfig.webapi_connection_config != null)
               {
                  session.user_id = AppWebClient.Instance.GetLoggedUserData().user_id;

                  result = await WebApiCall.Session.UpdateSession(AppWebClient.Instance.GetClient(), session);

                  OnNotificationShow?.Invoke(null, new NotificationEventArgs("Data synchronized successfully!", 3));
               }
               else if (appConfig.database_type == AppConfig.enDataBaseType.JSON && appConfig.json_database_config != null)
               {
                  var storedData = await LoadData();
                  if (storedData != null)
                  {
                     var rowEditable = storedData.sessions.Find(x => x.session_id == session.session_id);

                     rowEditable.start_date = session.start_date;
                     rowEditable.end_date = session.end_date;
                     rowEditable.description = session.description;
                     rowEditable.observation = session.observation;

                     await SaveAllSessions(storedData.sessions, OnNotificationShow);
                  }
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
               if (appConfig.database_type == AppConfig.enDataBaseType.WebApi && appConfig.webapi_connection_config != null)
               {
                  await WebApiCall.Session.DeleteSession(AppWebClient.Instance.GetClient(), sessionID);

                  OnNotificationShow?.Invoke(null, new NotificationEventArgs("Data synchronized successfully!", 3));
               }
               else if (appConfig.database_type == AppConfig.enDataBaseType.JSON && appConfig.json_database_config != null)
               {
                  var storedData = await LoadData();
                  var rowToRemove = storedData.sessions.RemoveAll(x => x.session_id == sessionID);

                  await SaveAllSessions(storedData.sessions, OnNotificationShow);
               }
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }
   }
}
