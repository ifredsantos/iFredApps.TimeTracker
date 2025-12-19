using iFredApps.TimeTracker.UI.Models;
using System.Globalization;
using System.Threading;
using System;
using System.Windows;
using iFredApps.TimeTracker.UI.Utils;
using AutoUpdaterDotNET;

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

         // Start AutoUpdater if update_feed_url is configured
         try
         {
            var feedUrl = SettingsLoader<AppConfig>.Instance.Data?.update_feed_url;
            if (!string.IsNullOrEmpty(feedUrl))
            {
               AutoUpdater.RunUpdateAsAdmin = false; // don't require elevation automatically
               AutoUpdater.ApplicationExitEvent += AutoUpdater_ApplicationExitEvent;
               AutoUpdater.CheckForUpdateEvent += AutoUpdater_CheckForUpdateEvent;
               AutoUpdater.Start(feedUrl);
            }
         }
         catch (Exception ex)
         {
            // If update check fails, ignore and continue starting the app
            // You can log this exception if you have logging
         }

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

      private void AutoUpdater_ApplicationExitEvent()
      {
         // AutoUpdater requested application exit to perform update
         Current.Dispatcher.Invoke(() => Current.Shutdown());
      }

      private void AutoUpdater_CheckForUpdateEvent(UpdateInfoEventArgs args)
      {
         // Optional: handle update info or customize behavior. Leave default if args.IsUpdateAvailable false
      }
   }
}
