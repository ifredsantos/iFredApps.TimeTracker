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
using MahApps.Metro.Controls;
using TimeTracker.UI.Models;

namespace TimeTracker.UI
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : MetroWindow
   {

      public MainWindow()
      {
         InitializeComponent();
      }

      private void LaunchGitHubSite(object sender, RoutedEventArgs e)
      {
         System.Diagnostics.Process.Start("https://github.com/ifredsantos");
      }

      private void Donation(object sender, RoutedEventArgs e)
      {

      }
   }
}
