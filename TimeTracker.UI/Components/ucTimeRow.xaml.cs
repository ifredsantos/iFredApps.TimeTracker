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
using TimeTracker.UI.Models;
using static System.Collections.Specialized.BitVector32;

namespace TimeTracker.UI.Components
{
    /// <summary>
    /// Interaction logic for ucTimeRow.xaml
    /// </summary>
    public partial class ucTimeRow : UserControl
    {
        public event EventHandler<TimeTaskContinueEventArgs> OnTaskContinue;
        public event EventHandler<TimeTaskRemoveEventArgs> OnTaskRemove;

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
    }
}
