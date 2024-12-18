using iFredApps.TimeTracker.UI.Models;
using iFredApps.Lib.Wpf.Execption;
using iFredApps.Lib;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Threading.Tasks;

namespace iFredApps.TimeTracker.UI.Views
{
   /// <summary>
   /// Interaction logic for ucWorkspacesView.xaml
   /// </summary>
   public partial class ucWorkspacesView : UserControl
   {
      private bool _isFirstLoadComplete = false;

      private List<Workspace> _workspaces = null;

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
            _workspaces = await WebApiCall.Workspaces.GetAllByUserId(AppWebClient.Instance.GetClient(), AppWebClient.Instance.GetLoggedUserData().user_id);

            DataContext = _workspaces;
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
   }
}
