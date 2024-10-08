using iFredApps.Lib.WebApi;
using Newtonsoft.Json;
using System;
using System.IO;

namespace TimeTracker.UI.Utils
{
   public class SettingsLoader<T> : Singleton<SettingsLoader<T>> where T : BaseSettingsData
   {
      public static string SettingsFileName { get; set; }

      static SettingsLoader()
      {
         SettingsFileName = "iFredApps.TimeTracker.config.json";
      }


      private bool m_loaded;
      private T m_data;

      public T Data
      {
         get
         {
            if (!m_loaded)
            {
               m_loaded = true;
               m_data = ReadJSONConfig();
            }

            return m_data;
         }
      }

      private static T ReadJSONConfig()
      {
         T data = default;

         //TODO: Confirmar se AppContext.BaseDirectory está correcto
         string _fileXML = Path.Combine(AppContext.BaseDirectory, SettingsFileName);
         if (File.Exists(_fileXML))
         {
            data = JsonConvert.DeserializeObject<T>(File.ReadAllText(_fileXML));
         }

         return data;
      }
   }

   public abstract class BaseSettingsData
   {
   }
}
