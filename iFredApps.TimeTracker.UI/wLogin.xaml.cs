using iFredApps.TimeTracker.UI.Models;
using iFredApps.TimeTracker.UI.Utils;
using iFredApps.TimeTracker.UI.ViewModels;
using System;
using System.Windows;
using System.Windows.Input;
using iFredApps.Lib.Wpf.Execption;
using iFredApps.Lib.Wpf.Messages;
using MahApps.Metro.Controls;

namespace iFredApps.TimeTracker.UI
{
   /// <summary>
   /// Lógica interna para wLogin.xaml
   /// </summary>
   public partial class wLogin : MetroWindow
   {
      private AppConfig appConfig;

      public wLogin()
      {
         InitializeComponent();

         PreviewKeyUp += UcLoginView_PreviewKeyUp;

         LoginBtn.Click += LoginBtn_Click;

         appConfig = SettingsLoader<AppConfig>.Instance.Data;
         if (appConfig.database_type == AppConfig.enDataBaseType.JSON)
         {
            OpenMainWindow();
            return;
         }

         txtUser.Focus();
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

      #region Private

      private async void LoginSubmit()
      {
         LoginVM loginVM = DataContext as LoginVM;
         try
         {
            loginVM.isLoading = true;

            loginVM.password = txtPassword.Password;

            if (string.IsNullOrEmpty(loginVM.user) || string.IsNullOrEmpty(loginVM.password))
            {
               Message.Warning("Enter credentials!");
               return;
            }

            AppWebClient.Instance.Address = SettingsLoader<AppConfig>.Instance.Data?.webapi_connection_config?.baseaddress;
            await AppWebClient.Instance.Login(loginVM.user, loginVM.password);
            if (AppWebClient.Instance.GetLoggedUserData() != null)
            {
               OpenMainWindow();
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
         finally
         {
            loginVM.isLoading = false;
         }
      }

      private void OpenMainWindow()
      {
         this.Hide();

         MainWindow mainWin = new MainWindow();
         bool? winResult = mainWin.ShowDialog();
         if (winResult == null || winResult.Value == false)
         {
            Application.Current.Shutdown();
         }
      }

      #endregion
   }
}
