using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using iFredApps.TimeTracker.UI.Models;
using iFredApps.TimeTracker.UI.Utils;
using iFredApps.TimeTracker.UI.Views;
using iFredApps.Lib.Wpf.Execption;
using System.Windows.Controls;

namespace iFredApps.TimeTracker.UI
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : MetroWindow
   {
      private bool _isLogoutWorking;

      public MainWindow()
      {
         InitializeComponent();

         _isLogoutWorking = false;

         InitMenu();

         cmpMenu.menuList.SelectedIndex = 0;

         Closed += MainWindow_Closed;
      }

      #region Methods

      private ucTimeManagerView GetTimeTrackerView()
      {
         ucTimeManagerView view = new ucTimeManagerView();
         view.OnNotificationShow += OnNotificationShow;

         return view;
      }

      private ucWorkspacesView GetWorkspaceView()
      {
         ucWorkspacesView view = new ucWorkspacesView();
         view.OnNotificationShow += OnNotificationShow;

         return view;
      }

      private void InitMenu()
      {
         AppMenu menu = new AppMenu();

         List<AppMenuItem> menuItemsList = new List<AppMenuItem>
         {
            new AppMenuItem("Time Tracker", PackIconFontAwesomeKind.ClockRegular, GetTimeTrackerView()),
            new AppMenuItem("Projects", PackIconFontAwesomeKind.TableColumnsSolid, new ucProjectsView()),
            new AppMenuItem("Workspaces", PackIconFontAwesomeKind.SpaceAwesomeBrands, GetWorkspaceView()),
            new AppMenuItem("Settings", PackIconFontAwesomeKind.GearSolid, new ucSettingsView()),
            //new AppMenu("Utils", PackIconFontAwesomeKind.CodeBranchSolid, new ucUtilitiesView())
         };

         menu.MenuList.AddRange(menuItemsList);
         menu.UserData = AppWebClient.Instance.GetLoggedUserData();

         cmpMenu.DataContext = menu;

         cmpMenu.menuList.SelectionChanged += MenuList_SelectionChanged;
      }

      public void ShowNotification(string message, TimeSpan? duration = null)
      {
         snackBar.MessageQueue.Enqueue(message, null, null, null, false, true, duration ?? TimeSpan.FromSeconds(5));
      }

      #endregion

      #region Events

      private void MainWindow_Closed(object sender, EventArgs e)
      {
         try
         {
            if (!_isLogoutWorking)
               Application.Current.Shutdown();
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private void MenuList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
      {
         try
         {
            if (e.AddedItems[0] is AppMenuItem menu)
            {
               if (menu.screen != null)
               {
                  contentControl.Content = menu.screen;
               }
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private void OnLogout(object sender, EventArgs e)
      {
         try
         {
            _isLogoutWorking = true;
            this.Close();

            if (Window.GetWindow(App.Current.MainWindow) is wLogin loginWin)
            {
               AppWebClient.Instance.Logout();
               loginWin.Clean();
               loginWin.Show();
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

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
