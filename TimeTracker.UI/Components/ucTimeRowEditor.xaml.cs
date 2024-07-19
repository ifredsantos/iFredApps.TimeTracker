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
        public bool isWorking
        {
            get { return (bool)GetValue(isWorkingProperty); }
            set { SetValue(isWorkingProperty, value); }
        }
        public static readonly DependencyProperty isWorkingProperty = DependencyProperty.Register("isWorking", typeof(bool), typeof(ucTimeRowEditor), new PropertyMetadata(false));


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
            if(DataContext is TimeManagerTaskSession session)
            {
                session.total_time = DateTime.Now - session.start_date;
            }
        }

        private void OnStartStopButton_Click(object sender, RoutedEventArgs e)
        {
            if (isWorking)
            {
                //End session
                if (DataContext is TimeManagerTaskSession session)
                {
                    session.end_date = DateTime.Now;
                    isWorking = false;
                    timer.Stop();

                    OnSessionChanged?.Invoke(this, new TimeRowChangedEventArgs { SessionData = session });
                }
            }
            else
            {
                //Start Session
                if (DataContext is TimeManagerTaskSession session)
                {
                    session.start_date = DateTime.Now;
                    isWorking = true;
                    timer.Start();
                }
            }
        }
    }
}
