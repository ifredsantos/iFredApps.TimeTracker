using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using iFredApps.TimeTracker.UI.Models;
using iFredApps.Lib.Wpf.Execption;
using iFredApps.Lib;
using System.Threading.Tasks;

namespace iFredApps.TimeTracker.UI.Views
{
   /// <summary>
   /// Interaction logic for ucTimeManager.xaml
   /// </summary>
   public partial class ucTimeManagerView : UserControl
   {
      public event EventHandler<NotificationEventArgs> OnNotificationShow;

      private TimeManagerBase _tmBase = null;
      private bool _isFirstLoadComplete = false;

      public ucTimeManagerView()
      {
         InitializeComponent();

         _tmBase = new TimeManagerBase();

         DataContext = _tmBase;

         Loaded += UcTimeManagerView_Loaded;
      }

      #region Private Methods

      private async Task InitData()
      {
         try
         {
            _tmBase.NotifyValue(nameof(_tmBase.isLoading), true);

            var lstWorkspaces = await WebApiCall.Workspaces.GetAllByUserId(AppWebClient.Instance.GetClient(), AppWebClient.Instance.GetLoggedUserData().user_id);
            if (!lstWorkspaces.IsNullOrEmpty())
            {
               foreach (var workspace in lstWorkspaces)
               {
                  TimeManagerWorkspace tmWorkspace = new TimeManagerWorkspace
                  {
                     workspace_id = workspace.workspace_id,
                     user_id = workspace.user_id,
                     name = workspace.name,
                     is_default = workspace.is_default,
                     created_at = workspace.created_at,
                     time_manager = new TimeManager(workspace)
                  };

                  _tmBase.workspaces.Add(tmWorkspace);
               }
               _tmBase.selected_workspace = _tmBase.workspaces.Where(x => x.is_default)?.FirstOrDefault();
            }

            _isFirstLoadComplete = true;
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
         finally
         {
            _tmBase.NotifyValue(nameof(_tmBase.isLoading), false);
         }
      }

      #endregion

      #region Events

      private async void UcTimeManagerView_Loaded(object sender, RoutedEventArgs e)
      {
         try
         {
            if (!_isFirstLoadComplete)
            {
               await InitData();
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private void ucTimeByWorkspace_OnNotificationShow(object sender, NotificationEventArgs e)
      {
         OnNotificationShow?.Invoke(this, e);
      }

      #endregion
   }
}
