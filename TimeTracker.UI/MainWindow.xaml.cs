using MahApps.Metro.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using TimeTracker.UI.Models;
using TimeTracker.UI.Utils;

namespace TimeTracker.UI
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : MetroWindow
   {
      public MainWindow()
      {
         InitializeComponent();
      }

      #region Buttons

      private void LaunchGitHubSite(object sender, RoutedEventArgs e)
      {
         try
         {
            Process.Start(new ProcessStartInfo
            {
               FileName = "https://github.com/ifredsantos",
               UseShellExecute = true,
            });
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private void Donation(object sender, RoutedEventArgs e)
      {
         try
         {
            ShowNotification("Available soon!");
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      #endregion

      #region Notification

      public void ShowNotification(string message, TimeSpan? duration = null)
      {
         try
         {
            snackBar.MessageQueue.Enqueue(message, null, null, null, false, true, duration ?? TimeSpan.FromSeconds(5));
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private void OnNotificationShow(object sender, NotificationEventArgs e)
      {
         try
         {
            if (string.IsNullOrEmpty(e.Message))
               return;

            ShowNotification(e.Message, e.Duration);
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      #endregion
   }
}
