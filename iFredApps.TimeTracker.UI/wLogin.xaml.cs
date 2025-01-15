using iFredApps.TimeTracker.UI.Models;
using iFredApps.TimeTracker.UI.Utils;
using System;
using System.Windows;
using System.Windows.Input;
using iFredApps.Lib.Wpf.Execption;
using iFredApps.Lib.Wpf.Messages;
using MahApps.Metro.Controls;
using iFredApps.Lib.Security;
using iFredApps.Lib;
using System.Diagnostics;

namespace iFredApps.TimeTracker.UI
{
   /// <summary>
   /// Lógica interna para wLogin.xaml
   /// </summary>
   public partial class wLogin : MetroWindow
   {
      private AppConfig appConfig;
      private LoginViewModel loginViewModel;

      public wLogin()
      {
         InitializeComponent();

         PreviewKeyUp += UcLoginView_PreviewKeyUp;

         LoginBtn.Click += LoginBtn_Click;

         appConfig = SettingsLoader<AppConfig>.Instance.Data;

         Clean();

         LoadData();
      }

      #region Events

      private void UcLoginView_PreviewKeyUp(object sender, KeyEventArgs e)
      {
         if (e.Key == Key.Enter)
         {
            LoginSubmit();
         }
      }

      private void LoginBtn_Click(object sender, RoutedEventArgs e)
      {
         LoginSubmit();
      }

      #endregion

      #region Public Methods

      public void Clean()
      {
         loginViewModel = new LoginViewModel();
         DataContext = loginViewModel;
         txtPassword.Password = null;

         txtUser.Focus();
      }

      #endregion

      #region Private

      private void LoadData()
      {
         try
         {
            SecurelyLocalData secData = new SecurelyLocalData(appConfig.SaveAppInfoDirectory);

            string loginSavedData = secData.LoadDataSecurely();
            if (!string.IsNullOrEmpty(loginSavedData))
            {
               string[] loginInfo = loginSavedData.Split(':');
               if (loginInfo.Length == 2)
               {
                  loginViewModel.user = loginInfo[0];
                  loginViewModel.password = loginInfo[1];
                  txtPassword.Password = loginInfo[1];
                  loginViewModel.savePassword = true;
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
            if (loginViewModel.isLoading)
               return;

            loginViewModel.NotifyValue(nameof(loginViewModel.isLoading), true);

            loginViewModel.password = txtPassword.Password;

            if (string.IsNullOrEmpty(loginViewModel.user) || string.IsNullOrEmpty(loginViewModel.password))
            {
               Message.Warning("Enter credentials!");
               return;
            }

            AppWebClient.Instance.Address = SettingsLoader<AppConfig>.Instance.Data?.webapi_connection_config?.baseaddress;
            await AppWebClient.Instance.Login(loginViewModel.user, loginViewModel.password);
            if (AppWebClient.Instance.GetLoggedUserData() != null)
            {
               SecurelyLocalData secData = new SecurelyLocalData(appConfig.SaveAppInfoDirectory);

               if (loginViewModel.savePassword)
               {
                  secData.SaveDataSecurely(string.Format("{0}:{1}", loginViewModel.user, loginViewModel.password));
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
            loginViewModel.NotifyValue(nameof(loginViewModel.isLoading), false);
         }
      }

      private void OpenMainWindow()
      {
         MainWindow mainWin = new MainWindow();
         mainWin.Show();
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
   }
}
