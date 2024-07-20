using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TimeTracker.UI.Models;

namespace TimeTracker.UI.Components
{
    /// <summary>
    /// Interação lógica para ucTimeRowEditor.xam
    /// </summary>
    public partial class ucTimeRowEditor : UserControl
    {
        public event EventHandler<TimeRowChangedEventArgs> OnSessionChanged;

        private DispatcherTimer timer;

        public ucTimeRowEditor()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += OnTimer_Tick;
        }

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
                    if (string.IsNullOrEmpty(currentSession.description))
                    {
                        MessageBox.Show("Cannot save a task without a description.", "Calm down!", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    currentSession.end_date = DateTime.Now;
                    currentSession.is_working = false;
                    timer.Stop();

                    OnSessionChanged?.Invoke(this, new TimeRowChangedEventArgs { SessionData = currentSession });
                }
                else
                {
                    currentSession.start_date = DateTime.Now;
                    currentSession.is_working = true;
                    timer.Start();
                }
            }
        }
    }
}
