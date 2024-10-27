using System;
using System.Windows;
using System.Windows.Controls;
using TimeTracker.UI.Models;

namespace TimeTracker.UI.Components
{
   /// <summary>
   /// Interaction logic for ucTimeRow.xaml
   /// </summary>
   public partial class ucTimeRow : UserControl
   {
      public event EventHandler<TimeTaskContinueEventArgs> OnTaskContinue;
      public event EventHandler<TimeTaskRemoveEventArgs> OnTaskRemove;
      public event EventHandler<TimeTaskEditEventArgs> OnTaskChanged;

      public ucTimeRow()
      {
         InitializeComponent();
      }

      #region Events

      private void OnTaskContinueClick(object sender, RoutedEventArgs e)
      {
         if (DataContext is TimeManagerTask taskData)
         {
            OnTaskContinue?.Invoke(this, new TimeTaskContinueEventArgs { TaskData = taskData });
         }
      }

      private void OnTaskRemoveClick(object sender, RoutedEventArgs e)
      {
         if (DataContext is TimeManagerTask taskData)
         {
            OnTaskRemove?.Invoke(this, new TimeTaskRemoveEventArgs { TaskData = taskData });
         }
      }

      private void OnDescriptionLostFocus(object sender, RoutedEventArgs e)
      {
         if (DataContext is TimeManagerTask taskData)
         {
            string oldDescription = taskData.description.ToString();
            taskData.description = ((TextBox)e.Source).Text.Trim();
            OnTaskChanged?.Invoke(this, new TimeTaskEditEventArgs { oldDescription = oldDescription, TaskData = taskData });
         }
      }

      private void OnDetailButtonClick(object sender, RoutedEventArgs e)
      {
         if (DataContext is TimeManagerTask taskData)
         {
            if (taskData.is_detail_session_open) //Close
            {
               detailButtonIcon.Kind = MahApps.Metro.IconPacks.PackIconBootstrapIconsKind.ChevronDown;
               sessionDetail.Visibility = Visibility.Collapsed;
            }
            else //Open
            {
               detailButtonIcon.Kind = MahApps.Metro.IconPacks.PackIconBootstrapIconsKind.ChevronUp;
               sessionDetail.Visibility = Visibility.Visible;
            }

            taskData.is_detail_session_open = !taskData.is_detail_session_open;
         }
      }

      #endregion

      private void OnEditSessionRow(object sender, RoutedEventArgs e)
      {

      }

      private void OnDeleteSessionRow(object sender, RoutedEventArgs e)
      {

      }
   }
}
