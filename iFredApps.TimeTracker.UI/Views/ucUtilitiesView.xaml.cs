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
using iFredApps.Lib.Wpf.Execption;

namespace iFredApps.TimeTracker.UI.Views
{
   /// <summary>
   /// Interação lógica para ucUtilities.xam
   /// </summary>
   public partial class ucUtilitiesView : UserControl
   {
      public ucUtilitiesView()
      {
         InitializeComponent();

         btnChooseFileDirectory.Click += BtnChooseFileDirectory_Click;
      }

      private void BtnChooseFileDirectory_Click(object sender, RoutedEventArgs e)
      {
         try
         {
            // Configure open file dialog box
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".json"; // Default file extension
            dialog.Filter = "JSON (.json)|*.json"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
               // Open document
               string filename = dialog.FileName;
               txtFileDirectory.Text = filename;
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }
   }
}
