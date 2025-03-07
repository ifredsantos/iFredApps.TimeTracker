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
      private LoginViewModel _loginViewModel;

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

      #endregion

      #region Public Methods

      public void Clean()
      {
         _loginViewModel = new LoginViewModel();
         DataContext = _loginViewModel;
         txtPassword.Password = null;

         txtUser.Focus();
      }

      #endregion

      #region Private

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
                  _loginViewModel.user = loginInfo[0];
                  _loginViewModel.password = loginInfo[1];
                  txtPassword.Password = loginInfo[1];
                  _loginViewModel.savePassword = true;
               }
            }
            else
            {
               txtUser.Focus();
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private async void LoginSubmit()
      {
         try
         {
            if (_loginViewModel.isLoading)
               return;

            _loginViewModel.NotifyValue(nameof(_loginViewModel.isLoading), true);

            _loginViewModel.password = txtPassword.Password;

            if (string.IsNullOrEmpty(_loginViewModel.user) || string.IsNullOrEmpty(_loginViewModel.password))
            {
               Message.Warning("Enter credentials!");
               return;
            }

            AppWebClient.Instance.Address = SettingsLoader<AppConfig>.Instance.Data?.webapi_connection_config?.baseaddress;
            await AppWebClient.Instance.Login(_loginViewModel.user, _loginViewModel.password);
            if (AppWebClient.Instance.GetLoggedUserData() != null)
            {
               SecurelyLocalData secData = new SecurelyLocalData(_appConfig.SaveAppInfoDirectory);

               if (_loginViewModel.savePassword)
               {
                  secData.SaveDataSecurely(string.Format("{0}:{1}", _loginViewModel.user, _loginViewModel.password));
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
            _loginViewModel.NotifyValue(nameof(_loginViewModel.isLoading), false);
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
