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

namespace TimeTracker.UI.Pages
{
   /// <summary>
   /// Interaction logic for ucTimeManager.xaml
   /// </summary>
   public partial class ucTimeManager : UserControl
   {
      #region Variables

      private TimeManager m_timeManager = new TimeManager();

      #endregion

      public ucTimeManager()
      {
         InitializeComponent();

         LoadStartInfo();

         DataContext = m_timeManager;

         lstView.ItemsSource = m_timeManager.task_groups;
      }

      #region Private Methods

      private void LoadStartInfo()
      {
         m_timeManager.LoadTasks();
      }

      #endregion
   }
}
