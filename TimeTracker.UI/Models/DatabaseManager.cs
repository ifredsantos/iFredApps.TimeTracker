using ControlzEx.Standard;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TimeTracker.UI.Pages;
using TimeTracker.UI.Utils;

namespace TimeTracker.UI.Models
{
   public static class DatabaseManager
   {
      public static async Task<TimeManagerDatabaseData> LoadData()
      {
         TimeManagerDatabaseData result = null;
         try
         {
            AppConfig appConfig = ((App)Application.Current)?.Config;

            if (appConfig != null)
            {
               if (appConfig.database_type == AppConfig.enDataBaseType.JSON && appConfig.json_database_config != null)
               {
                  string databaseFileDir = Path.Combine(appConfig.json_database_config.directory, appConfig.json_database_config.filename);

                  if (File.Exists(databaseFileDir))
                  {
                     string tasksJSON = File.ReadAllText(databaseFileDir);
                     result = JsonConvert.DeserializeObject<TimeManagerDatabaseData>(tasksJSON);
                  }
               }
               else if(appConfig.database_type == AppConfig.enDataBaseType.WebApi && appConfig.webapi_connection_config != null)
               {
                  var sessions = await new WebApiClient(appConfig.webapi_connection_config.baseaddress).GetAsync<List<TimeManagerTaskSession>>("Session?user_id={0}", 1);
                  result = new TimeManagerDatabaseData
                  {
                     sessions = sessions
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

      public static Task SaveAllSessions(TimeManagerDatabaseData sessions, EventHandler<NotificationEventArgs> OnNotificationShow)
      {
         return Task.Factory.StartNew(() =>
         {
            try
            {
               AppConfig appConfig = ((App)Application.Current).Config;
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
                     File.WriteAllText(databaseFileDir, tasksJSON);
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
   }
}
