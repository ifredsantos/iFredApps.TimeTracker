using iFredApps.TimeTracker.UI.Models;
using System.Globalization;
using System.Threading;
using System;
using System.Windows;

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
