using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using iFredApps.TimeTracker.UI.Models;
using iFredApps.Lib.Wpf.Execption;
using iFredApps.Lib;

namespace iFredApps.TimeTracker.UI.Components
{
   /// <summary>
   /// Interação lógica para ucTimeRowEditor.xam
   /// </summary>
   public partial class ucTimeRowEditor : UserControl
   {
      public event EventHandler<TimeRowSessionEventArgs> OnSessionStarts;
      public event EventHandler<TimeRowSessionEventArgs> OnSessionChanged;

      private DispatcherTimer timer;

      public ucTimeRowEditor()
      {
         InitializeComponent();

         timer = new DispatcherTimer();
         timer.Interval = TimeSpan.FromSeconds(1);
         timer.Tick += OnTimer_Tick;

         KeyUp += UcTimeRowEditor_KeyUp;
      }

      private void StartSession()
      {
         if (DataContext is TimeManagerTaskSession currentSession)
         {
            if (currentSession.start_date == DateTime.MinValue)
               currentSession.start_date = DateTime.Now;

            currentSession.NotifyValue(nameof(currentSession.is_working), true);

            timer.Start();

            if (string.IsNullOrEmpty(currentSession.description))
               currentSession.description = txtDescription.Text;

            OnSessionStarts?.Invoke(this, new TimeRowSessionEventArgs { SessionData = currentSession });
         }
      }

      private void TriggerDetail()
      {
         if (DataContext is TimeManagerTaskSession currentSession)
         {
            if (currentSession.is_detail_open)
            {
               detailButtonIcon.Kind = MahApps.Metro.IconPacks.PackIconBootstrapIconsKind.ChevronDown;
               sessionDetail.Visibility = Visibility.Collapsed;
            }
            else
            {
               detailButtonIcon.Kind = MahApps.Metro.IconPacks.PackIconBootstrapIconsKind.ChevronUp;
               sessionDetail.Visibility = Visibility.Visible;
            }

            currentSession.NotifyValue(nameof(currentSession.is_detail_open), !currentSession.is_detail_open);
         }
      }

      public void StartStopSession()
      {
         if (DataContext is TimeManagerTaskSession currentSession) //End session
         {
            if (currentSession.is_working)
            {
               if (currentSession.description == null)
                  currentSession.description = txtDescription.Text;

               if (currentSession.description != null)
                  currentSession.description = currentSession.description.TrimEnd(); //Remove empty spaces at the end

               if (string.IsNullOrEmpty(currentSession.description))
               {
                  MessageBox.Show("Cannot save a task without a description.", "Calm down!", MessageBoxButton.OK, MessageBoxImage.Warning);
                  return;
               }

               currentSession.end_date = DateTime.Now;
               currentSession.NotifyValue(nameof(currentSession.is_working), false);


               timer.Stop();

               OnSessionChanged?.Invoke(this, new TimeRowSessionEventArgs { SessionData = currentSession });

               TriggerDetail();
            }
            else
            {
               StartSession();
            }
         }
      }

      #region Events

      private void OnTimer_Tick(object sender, EventArgs e)
      {
         if (DataContext is TimeManagerTaskSession session)
         {
            session.NotifyValue(nameof(session.total_time), DateTime.Now - session.start_date);
         }
      }

      private void OnStartStopButton_Click(object sender, RoutedEventArgs e)
      {
         try
         {
            StartStopSession();
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private void UcTimeRowEditor_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
      {
         try
         {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
               StartStopSession();
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private void OnDetailButtonClick(object sender, RoutedEventArgs e)
      {
         try
         {
            TriggerDetail();
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      #endregion
   }
}
