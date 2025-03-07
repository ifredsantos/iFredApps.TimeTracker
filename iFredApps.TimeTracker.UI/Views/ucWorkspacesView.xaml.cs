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
using System.Net.WebSockets;
using iFredApps.TimeTracker.SL;

namespace iFredApps.TimeTracker.UI.Views
{
   /// <summary>
   /// Interaction logic for ucWorkspacesView.xaml
   /// </summary>
   public partial class ucWorkspacesView : UserControl
   {
      public event EventHandler<NotificationEventArgs> OnNotificationShow;

      private bool _isFirstLoadComplete = false;
      private hWorkspaceView _dataModel = new hWorkspaceView();

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
            _dataModel.workspaces.Clear();

            var resultWorkspaces = await WebApiCall.Workspaces.GetAllByUserId(AppWebClient.Instance.GetClient(), AppWebClient.Instance.GetLoggedUserData().user_id);
            var workspaces = resultWorkspaces?.TrataResposta();
            if (!workspaces.IsNullOrEmpty() && resultWorkspaces?.Success == true)
            {
               _dataModel.workspaces.AddRange(workspaces
                  .Select(w =>
                     new hWorkspace
                     {
                        workspace_id = w.workspace_id,
                        user_id = w.user_id,
                        name = w.name,
                        is_default = w.is_default,
                        created_at = w.created_at,
                        is_editing = false
                     })
                  .ToList());
            }

            DataContext = _dataModel;
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private async void SaveWorkspaceData(hWorkspace workspace)
      {
         bool isNew = workspace.workspace_id == 0;

         ApiResponse<sWorkspace> apiResponse = null;
         if (isNew)
         {
            apiResponse = await WebApiCall.Workspaces.Create(AppWebClient.Instance.GetClient(), workspace);
         }
         else
         {
            apiResponse = await WebApiCall.Workspaces.Update(AppWebClient.Instance.GetClient(), workspace);
         }

         var updatedWorkspace = apiResponse?.TrataResposta();
         if (apiResponse.Success && updatedWorkspace != null)
         {
            if (isNew)
               OnNotificationShow?.Invoke(this, new NotificationEventArgs("Workspace created successfully!"));
            else
               OnNotificationShow?.Invoke(this, new NotificationEventArgs("Workspace changed successfully!"));

            workspace.workspace_id = updatedWorkspace.workspace_id;
            workspace.is_editing = false;
            workspace.NotifyAll();
         }
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

      private async void OnDeleteWorkspace(object sender, System.Windows.RoutedEventArgs e)
      {
         try
         {
            if (Message.Confirmation("Are you sure you want to remove this workspace and all related sessions?") == MessageBoxResult.Yes)
            {
               if (e.Source is Button btn)
               {
                  if (btn.DataContext is hWorkspace workspace)
                  {
                     var apiResponse = await WebApiCall.Workspaces.Delete(AppWebClient.Instance.GetClient(), workspace.workspace_id.Value);
                     var sucesso = apiResponse?.TrataResposta();
                     if (sucesso.HasValue && sucesso.Value)
                     {
                        OnNotificationShow?.Invoke(this, new NotificationEventArgs("Workspace removed successfully!"));
                        _dataModel.workspaces.Remove(workspace);
                     }
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

      private void OnFavouriteWorkspaceChanged(object sender, RoutedEventArgs e)
      {
         try
         {
            if (e.Source is Button btn)
            {
               if (btn.DataContext is hWorkspace workspace)
               {
                  workspace.is_default = !workspace.is_default;

                  SaveWorkspaceData(workspace);

                  if (workspace.is_default)
                  {
                     foreach (var otherWorkspace in _dataModel.workspaces.Where(x => x.workspace_id != workspace.workspace_id))
                     {
                        otherWorkspace.NotifyValue(nameof(otherWorkspace.is_default), false);
                     }
                  }
               }
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private void OnAddNewWorkspace(object sender, RoutedEventArgs e)
      {
         try
         {
            _dataModel.workspaces.Add(new hWorkspace
            {
               user_id = AppWebClient.Instance.GetLoggedUserData().user_id,
               is_editing = true,
            });
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
         public IFAObservableCollection<hWorkspace> workspaces { get; set; }

         public hWorkspaceView()
         {
            workspaces = new IFAObservableCollection<hWorkspace>();
         }
      }

      private class hWorkspace : sWorkspace, INotifyPropertyChanged
      {
         public bool is_editing { get; set; }

         public event PropertyChangedEventHandler PropertyChanged;
      }

      #endregion
   }
}
