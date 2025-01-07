using iFredApps.TimeTracker.UI.Models;
using iFredApps.Lib.Wpf.Execption;
using iFredApps.Lib;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Linq;
using iFredApps.Lib.Wpf.Messages;
using System.Windows;
using System.ComponentModel;

namespace iFredApps.TimeTracker.UI.Views
{
   /// <summary>
   /// Interaction logic for ucWorkspacesView.xaml
   /// </summary>
   public partial class ucWorkspacesView : UserControl
   {
      public event EventHandler<NotificationEventArgs> OnNotificationShow;

      private bool _isFirstLoadComplete = false;

      private hWorkspaceView dataModel = new hWorkspaceView();

      public ucWorkspacesView()
      {
         InitializeComponent();

         Loaded += UcWorkspacesView_Loaded;
      }

      #region Private Methods

      private async Task LoadData()
      {
         try
         {
            var resultWorkspaces = await WebApiCall.Workspaces.GetAllByUserId(AppWebClient.Instance.GetClient(), AppWebClient.Instance.GetLoggedUserData().user_id);
            var workspaces = resultWorkspaces?.TrataResposta();
            if (!workspaces.IsNullOrEmpty() && resultWorkspaces?.Success == true)
            {
               dataModel.workspaces = workspaces
                  .Select(w => new hWorkspace { workspace_id = w.workspace_id, user_id = w.user_id, name = w.name, is_default = w.is_default, created_at = w.created_at, is_editing = false })
                  .ToList();
            }

            DataContext = dataModel;
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private void SaveWorkspaceData(hWorkspace workspace)
      {

      }

      #endregion

      #region Events

      private async void UcWorkspacesView_Loaded(object sender, System.Windows.RoutedEventArgs e)
      {
         try
         {
            if (!_isFirstLoadComplete)
            {
               await LoadData();
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private void OnEditWorkspace(object sender, System.Windows.RoutedEventArgs e)
      {
         try
         {
            if (e.Source is Button btn)
            {
               if (btn.DataContext is hWorkspace workspace)
               {
                  workspace.NotifyValue(nameof(workspace.is_editing), true);
               }
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private void OnDeleteWorkspace(object sender, System.Windows.RoutedEventArgs e)
      {
         try
         {
            if (Message.Confirmation("Are you sure you want to remove this workspace and all related sessions??") == MessageBoxResult.Yes)
            {
               if (e.Source is Button btn)
               {
                  if (btn.DataContext is hWorkspace workspace)
                  {
                     //WebApiCall.Workspaces
                     //Notificação?
                  }
               }
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private void OnSaveWorkspaceCancel(object sender, System.Windows.RoutedEventArgs e)
      {
         try
         {
            if (e.Source is Button btn)
            {
               if (btn.DataContext is hWorkspace workspace)
               {
                  workspace.NotifyValue(nameof(workspace.is_editing), false);
               }
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private void OnSaveWorkspaceChanges(object sender, System.Windows.RoutedEventArgs e)
      {
         try
         {
            if (e.Source is Button btn)
            {
               if (btn.DataContext is hWorkspace workspace)
               {
                  SaveWorkspaceData(workspace);
               }
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      #endregion


      #region Auxiliar Classes

      private class hWorkspaceView
      {
         public List<hWorkspace> workspaces { get; set; }
      }

      private class hWorkspace : Workspace, INotifyPropertyChanged
      {
         public bool is_editing { get; set; }

         public event PropertyChangedEventHandler PropertyChanged;
      }

      #endregion
   }
}
