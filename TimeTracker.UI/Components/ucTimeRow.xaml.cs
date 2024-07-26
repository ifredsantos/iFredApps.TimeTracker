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
                //taskData.description = ((TextBox)e.Source).Text;
                //OnTaskChanged?.Invoke(this, new TimeTaskRemoveEventArgs { TaskData = taskData });
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
    }
}
