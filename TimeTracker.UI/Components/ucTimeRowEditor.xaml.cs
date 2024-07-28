using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using TimeTracker.UI.Models;

namespace TimeTracker.UI.Components
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
        }

        public void StartSession()
        {
            if (DataContext is TimeManagerTaskCurrentSession currentSession)
            {
                if (currentSession.start_date == DateTime.MinValue)
                    currentSession.start_date = DateTime.Now;
                currentSession.is_working = true;
                timer.Start();

                OnSessionStarts?.Invoke(this, new TimeRowSessionEventArgs { SessionData = currentSession });
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

        private void OnStartStopButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is TimeManagerTaskCurrentSession currentSession) //End session
            {
                if (currentSession.is_working)
                {
                    currentSession.description = currentSession.description.TrimEnd(); //Remove empty spaces at the end

                    if (string.IsNullOrEmpty(currentSession.description))
                    {
                        MessageBox.Show("Cannot save a task without a description.", "Calm down!", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    currentSession.end_date = DateTime.Now;
                    currentSession.is_working = false;
                    timer.Stop();

                    OnSessionChanged?.Invoke(this, new TimeRowSessionEventArgs { SessionData = currentSession });
                }
                else
                {
                    StartSession();
                }
            }
        }

        #endregion
    }
}
