using iFredApps.Lib;
using iFredApps.Lib.Wpf.Execption;
using iFredApps.Lib.Wpf.Messages;
using iFredApps.TimeTracker.UI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace iFredApps.TimeTracker.UI.Views
{
   /// <summary>
   /// Interação lógica para ucUtilities.xam
   /// </summary>
   public partial class ucUtilitiesView : UserControl
   {
      public event EventHandler<NotificationEventArgs> OnNotificationShow;

      private hUtilitiesImportData _dataModel = null;

      public ucUtilitiesView()
      {
         InitializeComponent();

         btnChooseFileDirectory.Click += BtnChooseFileDirectory_Click;

         _dataModel = new hUtilitiesImportData();

         DataContext = _dataModel;
      }

      #region Events

      private void BtnChooseFileDirectory_Click(object sender, RoutedEventArgs e)
      {
         try
         {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.DefaultExt = ".json";
            dialog.Filter = "JSON (.json)|*.json";

            bool? result = dialog.ShowDialog();

            if (result == true)
            {
               string filename = dialog.FileName;
               _dataModel.FileDirectory = filename;
               _dataModel.IsReadyToImport = true;

               _dataModel.NotifyAll();
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
            if (string.IsNullOrEmpty(_dataModel.FileDirectory) || !File.Exists(_dataModel.FileDirectory))
            {
               Message.Error("The file path is not valid or does not exist.");
               return;
            }

            string fileContent = await File.ReadAllTextAsync(_dataModel.FileDirectory);
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
               //TODO: This process should be improved to sent a list
               foreach (var session in sessionList)
               {
                  try
                  {
                     session.user_id = userID;

                     await ImportSession(session);
                  }
                  catch (Exception ex)
                  {
                     ex.ShowException();
                  }
               }

               OnNotificationShow?.Invoke(null, new NotificationEventArgs("Data imported successfully!"));
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
            return WebApiCall.Sessions.CreateSession(AppWebClient.Instance.GetClient(), session);
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
