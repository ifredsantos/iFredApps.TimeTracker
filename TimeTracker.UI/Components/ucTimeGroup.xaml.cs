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

namespace TimeTracker.UI.Components
{
   /// <summary>
   /// Interaction logic for ucTimeGroup.xaml
   /// </summary>
   public partial class ucTimeGroup : UserControl
   {
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
   }
}
