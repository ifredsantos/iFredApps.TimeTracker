using iFredApps.Lib;
using iFredApps.Lib.Wpf.Execption;
using iFredApps.TimeTracker.UI.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TimeTracker.SL;

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

         tabWorkspaces.SelectionChanged += TabWorkspaces_SelectionChanged;
      }

      #region Private Methods

      private async Task InitData()
      {
         try
         {
            _tmBase.NotifyValue(nameof(_tmBase.isLoading), true);

            var resultLstWorkspaces = await WebApiCall.Workspaces.GetAllByUserId(AppWebClient.Instance.GetClient(), AppWebClient.Instance.GetLoggedUserData().user_id);
            var lstWorkspaces = resultLstWorkspaces?.TrataResposta();
            if (!lstWorkspaces.IsNullOrEmpty() && resultLstWorkspaces?.Success == true)
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
                     time_manager = new TimeManager(workspace),
                  };

                  await tmWorkspace.time_manager.LoadSessions();

                  _tmBase.workspaces.Add(tmWorkspace);
               }
               _tmBase.selected_workspace = _tmBase.workspaces.Where(x => x.is_default)?.FirstOrDefault();
            }

            if (_tmBase.selected_workspace != null)
            {
               tabWorkspaces.SelectedItem = _tmBase.selected_workspace;
            }
            else if (_tmBase.selected_workspace == null && !_tmBase.workspaces.IsNullOrEmpty())
            {
               tabWorkspaces.SelectedItem = _tmBase.workspaces.FirstOrDefault();
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

      private void TabWorkspaces_SelectionChanged(object sender, SelectionChangedEventArgs e)
      {
         try
         {
            if (e.AddedItems[0] is TimeManagerWorkspace workSpace)
            {
               _tmBase.selected_workspace = workSpace;
               _tmBase.NotifyValue(nameof(_tmBase.selected_workspace));
            }
         }
         catch (Exception ex)
         {
            Console.WriteLine(ex);
         }
      }

      #endregion
   }
}
