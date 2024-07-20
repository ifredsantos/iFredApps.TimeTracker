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
        public event EventHandler<TimeTaskRemoveEventArgs> OnTaskChanged;

        public ucTimeRow()
        {
            InitializeComponent();
        }

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
                taskData.description = ((TextBox)e.Source).Text;
                OnTaskChanged?.Invoke(this, new TimeTaskRemoveEventArgs { TaskData = taskData });
            }
        }
    }
}
