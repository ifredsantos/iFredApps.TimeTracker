using iFredApps.Lib;
using iFredApps.Lib.Security;
using iFredApps.Lib.Wpf.Execption;
using iFredApps.Lib.Wpf.Messages;
using iFredApps.TimeTracker.UI.Models;
using iFredApps.TimeTracker.UI.Utils;
using MahApps.Metro.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace iFredApps.TimeTracker.UI
{
   /// <summary>
   /// Lógica interna para wLogin.xaml
   /// </summary>
   public partial class wLogin : MetroWindow
   {
      private AppConfig _appConfig;
      private LoginViewModel _viewModel;

      public wLogin()
      {
         InitializeComponent();

         PreviewKeyUp += UcLoginView_PreviewKeyUp;

         LoginBtn.Click += LoginBtn_Click;

         _appConfig = SettingsLoader<AppConfig>.Instance.Data;

         Clean();

         LoadInitialData();
      }

      #region Events

      private void UcLoginView_PreviewKeyUp(object sender, KeyEventArgs e)
      {
         try
         {
            if (e.Key == Key.Enter)
            {
               LoginSubmit();
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private void LoginBtn_Click(object sender, RoutedEventArgs e)
      {
         try
         {
            LoginSubmit();
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private void OnLaunchGitHubSite(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
      {
         try
         {
            Process.Start(new ProcessStartInfo
            {
               FileName = "https://github.com/ifredsantos",
               UseShellExecute = true,
            });
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private void SignUp_Click(object sender, MouseEventArgs e)
      {
         this.Hide();

         wSignUp winSignUp = new wSignUp();
         winSignUp.ShowDialog();
          
         winSignUp.Close();
         this.Show();
      }

      #endregion

      #region Public Methods

      public void Clean()
      {
         _viewModel = new LoginViewModel();
         DataContext = _viewModel;
         txtPassword.Password = null;

         txtUser.Focus();
      }

      #endregion

      #region Private Methods

      private void LoadInitialData()
      {
         try
         {
            SecurelyLocalData secData = new SecurelyLocalData(_appConfig.SaveAppInfoDirectory);

            string loginSavedData = secData.LoadDataSecurely();
            if (!string.IsNullOrEmpty(loginSavedData))
            {
               string[] loginInfo = loginSavedData.Split(':');
               if (loginInfo.Length == 2)
               {
                  _viewModel.user = loginInfo[0];
                  _viewModel.password = loginInfo[1];
                  txtPassword.Password = loginInfo[1];
                  _viewModel.savePassword = true;
               }
            }
            else
            {
               txtUser.Focus();
            }
         }
         catch (Exception ex)
         {
            //ex.ShowException();
            Console.WriteLine(ex);
         }
      }

      private async void LoginSubmit()
      {
         try
         {
            if (_viewModel.isLoading)
               return;

            _viewModel.NotifyValue(nameof(_viewModel.isLoading), true);

            _viewModel.password = txtPassword.Password;

            if (string.IsNullOrEmpty(_viewModel.user) || string.IsNullOrEmpty(_viewModel.password))
            {
               Message.Warning("Enter credentials!");
               return;
            }

            await AppWebClient.Instance.Login(_viewModel.user, _viewModel.password);
            if (AppWebClient.Instance.GetLoggedUserData() != null)
            {
               SecurelyLocalData secData = new SecurelyLocalData(_appConfig.SaveAppInfoDirectory);

               if (_viewModel.savePassword)
               {
                  secData.SaveDataSecurely(string.Format("{0}:{1}", _viewModel.user, _viewModel.password));
               }
               else
               {
                  secData.DeleteSecureData();
               }

               this.Hide();
               OpenMainWindow();
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
         finally
         {
            _viewModel.NotifyValue(nameof(_viewModel.isLoading), false);
         }
      }

      private void OpenMainWindow()
      {
         MainWindow mainWin = new MainWindow();
         mainWin.Show();
      }

      #endregion
    }
}
