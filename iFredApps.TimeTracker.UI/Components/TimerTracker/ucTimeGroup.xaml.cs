using iFredApps.Lib.Wpf.Execption;
using iFredApps.TimeTracker.UI.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace iFredApps.TimeTracker.UI.Components
{
   /// <summary>
   /// Interaction logic for ucTimeGroup.xaml
   /// </summary>
   public partial class ucTimeGroup : UserControl
   {
      public event EventHandler<TimeTaskContinueEventArgs> OnTaskContinue;
      public event EventHandler<TimeTaskRemoveEventArgs> OnTaskRemove;
      public event EventHandler<TimeTaskRemoveEventArgs> OnTaskGetStatistics;
      public event EventHandler<TimeTaskEditEventArgs> OnTaskChanged;
      public event EventHandler<TimeTaskSessionEditEventArgs> OnSessionChanged;
      public event EventHandler<TimeTaskSessionEditEventArgs> OnSessionRemoved;
      public event EventHandler<TimeTaskGroupArgs> OnSendReportRequest;
      public event EventHandler<TimeTaskGroupArgs> OnDownloadReportRequest;

      public ucTimeGroup()
      {
         InitializeComponent();
      }

      #region Events

      private void lstView_PreviewMouseWheel(object sender, MouseWheelEventArgs e) //Disable scroll on list view
      {
         try
         {
            e.Handled = true;
            MouseWheelEventArgs e2 = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
            e2.RoutedEvent = UIElement.MouseWheelEvent;
            lstView.RaiseEvent(e2);
         }
         catch (Exception ex)
         {
            Console.WriteLine(ex);
         }
      }

      private void OnTaskContinueClick(object sender, TimeTaskContinueEventArgs e)
      {
         try
         {
            OnTaskContinue?.Invoke(this, e);
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private void OnTaskRemoveClick(object sender, TimeTaskRemoveEventArgs e)
      {
         try
         {
            OnTaskRemove?.Invoke(this, e);
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private void OnTaskGetStatisticsClick(object sender, TimeTaskRemoveEventArgs e)
      {
         try
         {
            OnTaskGetStatistics?.Invoke(this, e);
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private void OnTaskChange(object sender, TimeTaskEditEventArgs e)
      {
         try
         {
            OnTaskChanged?.Invoke(this, e);
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private void OnSessionChange(object sender, TimeTaskSessionEditEventArgs e)
      {
         try
         {
            OnSessionChanged?.Invoke(this, e);
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private void OnSessionRemove(object sender, TimeTaskSessionEditEventArgs e)
      {
         try
         {
            OnSessionRemoved?.Invoke(this, e);
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private void OnSendReport(object sender, RoutedEventArgs e)
      {
         try
         {
            if (DataContext is TimeManagerGroup group)
            {
               OnSendReportRequest?.Invoke(this, new TimeTaskGroupArgs { Group = group });
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      #endregion

      private void OnDownloadReport(object sender, RoutedEventArgs e)
      {
         try
         {
            if (DataContext is TimeManagerGroup group)
            {
               OnDownloadReportRequest?.Invoke(this, new TimeTaskGroupArgs { Group = group });
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }
   }
}
