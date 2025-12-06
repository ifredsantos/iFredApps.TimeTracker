using iFredApps.Lib;
using iFredApps.Lib.Security;
using iFredApps.Lib.Wpf.Execption;
using iFredApps.Lib.Wpf.Messages;
using iFredApps.TimeTracker.UI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace iFredApps.TimeTracker.UI
{
   /// <summary>
   /// Interaction logic for wRecuperarPassword.xaml
   /// </summary>
   public partial class wRecuperarPassword : Window
   {
      private RecuperarPasswordViewModel _viewModel = null;

      public wRecuperarPassword()
      {
         InitializeComponent();

         AvancarBtn.Click += AvancarBtn_Click;

         Clean();
      }

      #region Events

      private void AvancarBtn_Click(object sender, RoutedEventArgs e)
      {
         try
         {
            if (string.IsNullOrEmpty(_viewModel.email))
               EfetuarPedidoRecuperacao();
            else if (string.IsNullOrEmpty(_viewModel.codigo))
               EfetuarResetPassword();
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      #endregion

      #region Private Methods

      private void Clean()
      {
         _viewModel = new RecuperarPasswordViewModel();
         DataContext = _viewModel;
         //txtPassword.Password = null;

         txtEmail.Focus();
      }

      private void EfetuarPedidoRecuperacao()
      {

         try
         {
            if (_viewModel.isLoading)
               return;

            //_viewModel.NotifyValue(nameof(_viewModel.isLoading), true);

            //_viewModel.password = txtPassword.Password;

            //if (string.IsNullOrEmpty(_viewModel.user) || string.IsNullOrEmpty(_viewModel.password))
            //{
            //   Message.Warning("Enter credentials!");
            //   return;
            //}

            //await AppWebClient.Instance.Login(_viewModel.user, _viewModel.password);
            //if (AppWebClient.Instance.GetLoggedUserData() != null)
            //{
            //   SecurelyLocalData secData = new SecurelyLocalData(_appConfig.SaveAppInfoDirectory);

            //   if (_viewModel.savePassword)
            //   {
            //      secData.SaveDataSecurely(string.Format("{0}:{1}", _viewModel.user, _viewModel.password));
            //   }
            //   else
            //   {
            //      secData.DeleteSecureData();
            //   }

            //   this.Hide();
            //   OpenMainWindow();
            //}
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

      private void EfetuarResetPassword()
      {

      }

      #endregion
   }
}
