using iFredApps.TimeTracker.UI.Utils;
using Microsoft.VisualBasic.FileIO;
using System.IO;

namespace iFredApps.TimeTracker.UI.Models
{
   public class AppConfig : BaseSettingsData
   {
      public WebApiConnectionConfig webapi_connection_config { get; set; }
      public string SaveAppInfoDirectory { get; set; }

      public AppConfig()
      {
         SaveAppInfoDirectory = "securedata.dat";
      }
   }

   public class WebApiConnectionConfig
   {
      public string baseaddress { get; set; }
   }
}
