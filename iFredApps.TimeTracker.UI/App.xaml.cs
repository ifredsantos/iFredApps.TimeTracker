using iFredApps.TimeTracker.UI.Models;
using System.Globalization;
using System.Threading;
using System;
using System.Windows;
using iFredApps.TimeTracker.UI.Utils;

namespace iFredApps.TimeTracker.UI
{
   /// <summary>
   /// Interaction logic for App.xaml
   /// </summary>
   public partial class App : Application
   {
      public App()
      {
         InitializeComponent();
      }

      protected override void OnStartup(StartupEventArgs e)
      {
         base.OnStartup(e);

         AppWebClient.Instance.Init(SettingsLoader<AppConfig>.Instance.Data?.webapi_connection_config?.baseaddress);

         if (AppWebClient.Instance.GetLoggedUserData() != null)
         {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
         }
         else
         {
            wLogin loginWindow = new wLogin();
            loginWindow.Show();
         }
      }
   }
}
