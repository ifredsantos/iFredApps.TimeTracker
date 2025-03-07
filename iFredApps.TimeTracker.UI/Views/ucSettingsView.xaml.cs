using iFredApps.Lib.Wpf.Execption;
using iFredApps.TimeTracker.UI.Models;
using System;
using System.Windows.Controls;

namespace iFredApps.TimeTracker.UI.Views
{
   /// <summary>
   /// Interaction logic for ucSettingsView.xaml
   /// </summary>
   public partial class ucSettingsView : UserControl
   {
      public event EventHandler<NotificationEventArgs> OnNotificationShow;

      public ucSettingsView()
      {
         InitializeComponent();

         Loaded += UcSettingsView_Loaded;
      }

      private void UcSettingsView_Loaded(object sender, System.Windows.RoutedEventArgs e)
      {
         try
         {
            OnNotificationShow?.Invoke(null, new NotificationEventArgs("Available soon!", 3));
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }
   }
}
