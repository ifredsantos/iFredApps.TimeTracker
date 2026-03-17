using iFredApps.Lib;
using iFredApps.Lib.Wpf.Execption;
using iFredApps.TimeTracker.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
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
      private SessionSearchViewModel _searchViewModel = new SessionSearchViewModel();

      public ucTimeManagerView()
      {
         InitializeComponent();

         _tmBase = new TimeManagerBase();

         DataContext = _tmBase;
         searchPanel.DataContext = _searchViewModel;

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

      private void OpenSearchPanel()
      {
         searchOverlay.Visibility = Visibility.Visible;

         var animation = new DoubleAnimation
         {
            From = 400,
            To = 0,
            Duration = TimeSpan.FromMilliseconds(260),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
         };
         searchPanelTransform.BeginAnimation(TranslateTransform.XProperty, animation);
         txtSearchQuery.Focus();
      }

      private void CloseSearchPanel()
      {
         var animation = new DoubleAnimation
         {
            From = 0,
            To = 400,
            Duration = TimeSpan.FromMilliseconds(200),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
         };
         animation.Completed += (s, e) => searchOverlay.Visibility = Visibility.Collapsed;
         searchPanelTransform.BeginAnimation(TranslateTransform.XProperty, animation);
      }

      private async Task ExecuteSearch()
      {
         string query = txtSearchQuery.Text?.Trim();
         if (string.IsNullOrEmpty(query))
            return;

         _searchViewModel.NotifyValue(nameof(_searchViewModel.is_searching), true);
         _searchViewModel.NotifyValue(nameof(_searchViewModel.show_results), false);
         _searchViewModel.NotifyValue(nameof(_searchViewModel.show_no_results), false);
         _searchViewModel.results.Clear();

         try
         {
            var allResults = new List<SessionSearchResultItem>();
            int userId = AppWebClient.Instance.GetLoggedUserData().user_id;

            foreach (var workspace in _tmBase.workspaces)
            {
               var request = new GetSessionsRequest
               {
                  user_id = userId,
                  workspace_id = workspace.workspace_id.Value,
                  description = query
               };

               var response = await WebApiCall.Sessions.GetSessionsByDescription(
                  AppWebClient.Instance.GetClient(), request);

               if (response?.Success == true && !response.Data.IsNullOrEmpty())
               {
                  var grouped = response.Data
                     .Where(s => s.end_date.HasValue)
                     .GroupBy(s => s.description)
                     .Select(g => new SessionSearchResultItem
                     {
                        description = g.Key,
                        workspace_name = workspace.name,
                        session_count = g.Count(),
                        total_time = g.Aggregate(TimeSpan.Zero, (acc, s) =>
                           acc + (s.end_date.Value - s.start_date))
                     });

                  allResults.AddRange(grouped);
               }
            }

            if (allResults.Count > 0)
            {
               _searchViewModel.results.AddRange(allResults.OrderByDescending(r => r.total_time));
               _searchViewModel.NotifyValue(nameof(_searchViewModel.grand_total),
                  allResults.Aggregate(TimeSpan.Zero, (acc, r) => acc + r.total_time));
               _searchViewModel.NotifyValue(nameof(_searchViewModel.grand_total_display));
               _searchViewModel.NotifyValue(nameof(_searchViewModel.show_results), true);
            }
            else
            {
               _searchViewModel.NotifyValue(nameof(_searchViewModel.show_no_results), true);
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
         finally
         {
            _searchViewModel.NotifyValue(nameof(_searchViewModel.is_searching), false);
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

      private void btnOpenSearch_Click(object sender, RoutedEventArgs e)
      {
         OpenSearchPanel();
      }

      private void btnCloseSearch_Click(object sender, RoutedEventArgs e)
      {
         CloseSearchPanel();
      }

      private void SearchOverlay_MouseDown(object sender, MouseButtonEventArgs e)
      {
         CloseSearchPanel();
      }

      private async void btnSearch_Click(object sender, RoutedEventArgs e)
      {
         try
         {
            await ExecuteSearch();
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private async void txtSearchQuery_KeyDown(object sender, KeyEventArgs e)
      {
         if (e.Key == Key.Enter)
         {
            try
            {
               await ExecuteSearch();
            }
            catch (Exception ex)
            {
               ex.ShowException();
            }
         }
      }

      #endregion
   }
}
