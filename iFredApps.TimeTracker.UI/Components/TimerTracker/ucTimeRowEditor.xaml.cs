using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using iFredApps.TimeTracker.UI.Models;
using iFredApps.Lib.Wpf.Execption;
using iFredApps.Lib;
using System.Threading.Tasks;
using System.Threading;

namespace iFredApps.TimeTracker.UI.Components
{
   /// <summary>
   /// Interação lógica para ucTimeRowEditor.xam
   /// </summary>
   public partial class ucTimeRowEditor : UserControl
   {
      public event EventHandler<TimeRowSessionEventArgs> OnSessionStarts;
      public event EventHandler<TimeRowSessionEventArgs> OnSessionChanged;

      private CancellationTokenSource _timerCancellationTokenSource;

      public ucTimeRowEditor()
      {
         InitializeComponent();
         Loaded += UcTimeRowEditor_Loaded;
      }

      private void StartSession()
      {
         if (DataContext is TimeManagerTaskSession currentSession)
         {
            if (currentSession.start_date == DateTime.MinValue)
               currentSession.start_date = DateTime.Now;

            currentSession.NotifyValue(nameof(currentSession.is_working), true);

            if (string.IsNullOrEmpty(currentSession.description))
               currentSession.description = txtDescription.Text;

            OnSessionStarts?.Invoke(this, new TimeRowSessionEventArgs { SessionData = currentSession });

            StartTimerAsync(currentSession);
         }
      }

      private async void StartTimerAsync(TimeManagerTaskSession session)
      {
         StopTimer(); // Garante que não há timers ativos

         _timerCancellationTokenSource = new CancellationTokenSource();
         var token = _timerCancellationTokenSource.Token;

         try
         {
            while (!token.IsCancellationRequested && session.is_working)
            {
               await Task.Delay(1000, token);

               await Application.Current.Dispatcher.BeginInvoke((Action)(() =>
               {
                  if (!token.IsCancellationRequested && session.is_working)
                  {
                     session.NotifyValue(nameof(session.total_time), DateTime.Now - session.start_date);
                  }
               }), DispatcherPriority.Background);
            }
         }
         catch (TaskCanceledException)
         {
            // O timer foi cancelado, então não faz nada
         }
      }

      private void StopTimer()
      {
         Interlocked.Exchange(ref _timerCancellationTokenSource, null)?.Cancel();
      }

      public void StartStopSession()
      {
         if (DataContext is TimeManagerTaskSession currentSession)
         {
            if (currentSession.is_working) // Encerrar sessão
            {
               if (string.IsNullOrWhiteSpace(currentSession.description))
               {
                  MessageBox.Show("Cannot save a task without a description.", "Calm down!", MessageBoxButton.OK, MessageBoxImage.Warning);
                  return;
               }

               currentSession.end_date = DateTime.Now;
               currentSession.NotifyValue(nameof(currentSession.is_working), false);

               StopTimer();
               OnSessionChanged?.Invoke(this, new TimeRowSessionEventArgs { SessionData = currentSession });

               CloseDetail();
            }
            else
            {
               StartSession();
            }
         }
      }

      private void TriggerDetail()
      {
         if (DataContext is TimeManagerTaskSession currentSession)
         {
            if (currentSession.is_detail_open)
            {
               CloseDetail();
            }
            else
            {
               OpenDetail();
            }
         }
      }

      private void CloseDetail()
      {
         if (DataContext is TimeManagerTaskSession currentSession)
         {
            detailButtonIcon.Kind = MahApps.Metro.IconPacks.PackIconBootstrapIconsKind.ChevronDown;
            sessionDetail.Visibility = Visibility.Collapsed;
            currentSession.NotifyValue(nameof(currentSession.is_detail_open), false);
         }
      }

      private void OpenDetail()
      {
         if (DataContext is TimeManagerTaskSession currentSession)
         {
            detailButtonIcon.Kind = MahApps.Metro.IconPacks.PackIconBootstrapIconsKind.ChevronUp;
            sessionDetail.Visibility = Visibility.Visible;
            currentSession.NotifyValue(nameof(currentSession.is_detail_open), true);
         }
      }

      #region Events

      private void UcTimeRowEditor_Loaded(object sender, RoutedEventArgs e)
      {
         try
         {
            if (DataContext is TimeManagerTaskSession currentSession && currentSession.is_working)
            {
               StartTimerAsync(currentSession);
            }
            KeyUp += UcTimeRowEditor_KeyUp;
            Unloaded += UserControl_Unloaded;
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private void UserControl_Unloaded(object sender, RoutedEventArgs e)
      {
         try
         {
            StopTimer();
            KeyUp -= UcTimeRowEditor_KeyUp;
            Unloaded -= UserControl_Unloaded;
         }
         catch (Exception ex)
         {
            ex.ShowException();
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
