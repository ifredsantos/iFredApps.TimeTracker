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
      public static List<TimeManagerTaskSession> LoadData()
      {
         List<TimeManagerTaskSession> result = null;
         try
         {
            AppConfig appConfig = ((App)Application.Current)?.Config;

            if (appConfig != null && appConfig.database_type == AppConfig.enDataBaseType.JSON && appConfig.json_database_config != null)
            {
               string directory = appConfig.json_database_config.directory;
               string filename = appConfig.json_database_config.filename;
               string databaseFileDir = Path.Combine(directory, filename);

               if (File.Exists(databaseFileDir))
               {
                  string tasksJSON = File.ReadAllText(databaseFileDir);
                  result = JsonConvert.DeserializeObject<List<TimeManagerTaskSession>>(tasksJSON);
               }
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }

         return result;
      }

      public static void SaveTasks(List<TimeManagerTaskSession> sessions, EventHandler<NotificationEventArgs> OnNotificationShow)
      {
         Task.Factory.StartNew(() =>
         {
            try
            {
               AppConfig appConfig = ((App)Application.Current).Config;

               string directory = appConfig.json_database_config.directory;
               string filename = appConfig.json_database_config.filename;
               string databaseFileDir = Path.Combine(directory, filename);

               if (!Directory.Exists(directory))
                  Directory.CreateDirectory(directory);

               string tasksJSON = JsonConvert.SerializeObject(sessions);
               File.WriteAllText(databaseFileDir, tasksJSON);
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
