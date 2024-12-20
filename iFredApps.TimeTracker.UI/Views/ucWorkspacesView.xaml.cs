using iFredApps.TimeTracker.UI.Models;
using iFredApps.Lib.Wpf.Execption;
using iFredApps.Lib;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;

namespace iFredApps.TimeTracker.UI.Views
{
   /// <summary>
   /// Interaction logic for ucWorkspacesView.xaml
   /// </summary>
   public partial class ucWorkspacesView : UserControl
   {
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
            var workspaces = await WebApiCall.Workspaces.GetAllByUserId(AppWebClient.Instance.GetClient(), AppWebClient.Instance.GetLoggedUserData().user_id);
            if(!workspaces.IsNullOrEmpty())
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

      #endregion


      #region Auxiliar Classes

      private class hWorkspaceView
      {
         public List<hWorkspace> workspaces { get; set; }
      }

      private class hWorkspace : Workspace
      {
         public bool is_editing { get; set; }
      }

      #endregion
   }
}
