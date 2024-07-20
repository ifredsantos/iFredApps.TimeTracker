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

namespace TimeTracker.UI.Components
{
    /// <summary>
    /// Interaction logic for ucTimeGroup.xaml
    /// </summary>
    public partial class ucTimeGroup : UserControl
    {
        public event EventHandler<TimeTaskContinueEventArgs> OnTaskContinue;
        public event EventHandler<TimeTaskRemoveEventArgs> OnTaskRemove;
        public event EventHandler<TimeTaskRemoveEventArgs> OnTaskChanged;

        public ucTimeGroup()
        {
            InitializeComponent();
        }

        private void lstView_PreviewMouseWheel(object sender, MouseWheelEventArgs e) //Disable scroll on list view
        {
            e.Handled = true;
            MouseWheelEventArgs e2 = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
            e2.RoutedEvent = UIElement.MouseWheelEvent;
            lstView.RaiseEvent(e2);
        }

        private void OnTaskContinueClick(object sender, TimeTaskContinueEventArgs e)
        {
            OnTaskContinue?.Invoke(this, e);
        }

        private void OnTaskRemoveClick(object sender, TimeTaskRemoveEventArgs e)
        {
            OnTaskRemove?.Invoke(this, e);
        }

        private void OnTaskChange(object sender, TimeTaskRemoveEventArgs e)
        {
            OnTaskChanged?.Invoke(this, e);
        }
    }
}
