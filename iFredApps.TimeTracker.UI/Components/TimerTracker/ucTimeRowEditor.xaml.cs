using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using iFredApps.TimeTracker.UI.Models;
using iFredApps.Lib.Wpf.Execption;
using System.Threading.Tasks;

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
         if (DataContext is TimeManagerTaskCurrentSession currentSession)
         {
            if (currentSession.start_date == DateTime.MinValue)
               currentSession.start_date = DateTime.Now;
            currentSession.is_working = true;
            timer.Start();

            if (string.IsNullOrEmpty(currentSession.description))
               currentSession.description = txtDescription.Text;

            OnSessionStarts?.Invoke(this, new TimeRowSessionEventArgs { SessionData = currentSession });
         }
      }

      public async Task StartStopSession()
      {
         if (DataContext is TimeManagerTaskCurrentSession currentSession) //End session
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
               currentSession.is_working = false;
               timer.Stop();

               var savedSessionData = await DatabaseManager.UpdateSession(currentSession);
               if(savedSessionData != null)
               {
                  currentSession.end_date = savedSessionData.end_date;
                  OnSessionChanged?.Invoke(this, new TimeRowSessionEventArgs { SessionData = savedSessionData });
               }
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
         if (DataContext is TimeManagerTaskCurrentSession session)
         {
            session.total_time = DateTime.Now - session.start_date;
         }
      }

      private async void OnStartStopButton_Click(object sender, RoutedEventArgs e)
      {
         try
         {
            await StartStopSession();
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private async void UcTimeRowEditor_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
      {
         try
         {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
               await StartStopSession();
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      #endregion
   }
}
