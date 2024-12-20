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
      private AppConfig appConfig;

      public MainWindow()
      {
         InitializeComponent();

         appConfig = SettingsLoader<AppConfig>.Instance.Data;

         InitMenu();

         SetTimeTrackerView();
      }

      #region Methods

      private void SetTimeTrackerView()
      {
         cmpMenu.menuList.SelectedIndex = 0;
      }

      private ucTimeManagerView GetTimeTrackerView()
      {
         ucTimeManagerView timeManagerView = new ucTimeManagerView();
         timeManagerView.OnNotificationShow += OnNotificationShow;

         return timeManagerView;
      }

      #endregion

      #region Menu

      private void InitMenu()
      {
         List<AppMenu> menus = new List<AppMenu>
         {
            new AppMenu("Time Tracker", PackIconFontAwesomeKind.ClockRegular, GetTimeTrackerView()),
            new AppMenu("Projects", PackIconFontAwesomeKind.TableColumnsSolid, new ucProjectsView()),
            new AppMenu("Workspaces", PackIconFontAwesomeKind.SpaceAwesomeBrands, new ucWorkspacesView()),
            new AppMenu("Settings", PackIconFontAwesomeKind.GearSolid, new ucSettingsView()),
            //new AppMenu("Utils", PackIconFontAwesomeKind.CodeBranchSolid, new ucUtilitiesView())
         };

         cmpMenu.menuList.ItemsSource = menus;

         cmpMenu.menuList.SelectionChanged += MenuList_SelectionChanged;
      }

      private void MenuList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
      {
         try
         {
            if (e.AddedItems[0] is AppMenu menu)
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

      #endregion

      #region Events

      #endregion

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
