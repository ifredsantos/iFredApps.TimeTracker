using System;
using System.Diagnostics;
using System.Windows.Controls;
using iFredApps.Lib.Wpf.Execption;

namespace iFredApps.TimeTracker.UI.Components
{
   /// <summary>
   /// Interaction logic for LateralMenu.xaml
   /// </summary>
   public partial class ucLateralMenu : UserControl
   {
      public event EventHandler<EventArgs> OnLogoutButtonClick;

      public ucLateralMenu()
      {
         InitializeComponent();
      }

      #region Events

      private void OnLogoutButton_Click(object sender, System.Windows.RoutedEventArgs e)
      {
         try
         {
            OnLogoutButtonClick?.Invoke(this, e);
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private void OnLaunchGitHubSite(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
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

      #endregion
   }
}
