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


      public ucTimeRowEditor()
      {
         InitializeComponent();
      }

      private void OnStartStopButton_Click(object sender, RoutedEventArgs e)
      {
         if (isWorking)
         {
            if (DataContext is TimeManagerTaskSession session)
            {
               session.end_date = DateTime.Now;

               OnSessionChanged?.Invoke(this, new TimeRowChangedEventArgs { SessionData = session });

               isWorking = false;
            }
         }
         else
         {

            if (DataContext is TimeManagerTaskSession session)
            {
               session.start_date = DateTime.Now;
               isWorking = true;
            }
         }
      }
   }
}
