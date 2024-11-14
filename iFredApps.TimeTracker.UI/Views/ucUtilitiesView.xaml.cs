using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
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
using iFredApps.Lib;
using iFredApps.Lib.Wpf.Execption;
using iFredApps.Lib.Wpf.Messages;
using iFredApps.TimeTracker.UI.Models;

namespace iFredApps.TimeTracker.UI.Views
{
   /// <summary>
   /// Interação lógica para ucUtilities.xam
   /// </summary>
   public partial class ucUtilitiesView : UserControl
   {
      private hUtilitiesImportData m_dataModel = null;

      public ucUtilitiesView()
      {
         InitializeComponent();

         btnChooseFileDirectory.Click += BtnChooseFileDirectory_Click;

         m_dataModel = new hUtilitiesImportData();

         DataContext = m_dataModel;
      }

      #region Events

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
               m_dataModel.FileDirectory = filename;
               m_dataModel.IsReadyToImport = true;

               m_dataModel.NotifyAll();
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private async void btnStartProcess_Click(object sender, RoutedEventArgs e)
      {
         try
         {
            if (string.IsNullOrEmpty(m_dataModel.FileDirectory) || !File.Exists(m_dataModel.FileDirectory))
            {
               Message.Error("The file path is not valid or does not exist.");
               return;
            }

            string fileContent = await File.ReadAllTextAsync(m_dataModel.FileDirectory);
            if (string.IsNullOrEmpty(fileContent))
            {
               Message.Error("The file path is not valid or does not exist.");
               return;
            }

            TimeManagerDatabaseData fileDataToImport = JsonSerializer.Deserialize<TimeManagerDatabaseData>(fileContent);
            List<TimeManagerTaskSession> sessionList = null;
            if (fileDataToImport != null)
               sessionList = fileDataToImport.sessions;

            int userID = AppWebClient.Instance.GetLoggedUserData().user_id;

            if (sessionList != null)
            {
               foreach (var session in sessionList)
               {
                  try
                  {
                     session.user_id = userID;

                     await ImportSession(session);
                  }
                  catch (Exception ex)
                  {
                     //TODO: Work show errors system
                  }
               }

               Message.Success("Data imported successfully!");
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private Task ImportSession(TimeManagerTaskSession session)
      {
         try
         {
            return WebApiCall.Session.CreateSession(AppWebClient.Instance.GetClient(), session);
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }

         return Task.CompletedTask;
      }

      #endregion

      #region Helper classes

      private class hUtilitiesImportData : INotifyPropertyChanged
      {
         public string FileDirectory { get; set; }
         public bool IsReadyToImport { get; set; }

         public event PropertyChangedEventHandler PropertyChanged;
      }

      #endregion
   }
}
