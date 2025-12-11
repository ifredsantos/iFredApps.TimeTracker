using iFredApps.Lib;
using iFredApps.Lib.Security;
using iFredApps.Lib.Wpf.Execption;
using iFredApps.Lib.Wpf.Messages;
using iFredApps.TimeTracker.UI.Models;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using static iFredApps.TimeTracker.UI.Models.RecuperarPasswordViewModel;

namespace iFredApps.TimeTracker.UI
{
   /// <summary>
   /// Interaction logic for wRecuperarPassword.xaml
   /// </summary>
   public partial class wRecuperarPassword : MetroWindow
   {
      private RecuperarPasswordViewModel _viewModel = null;
      

      public wRecuperarPassword()
      {
         InitializeComponent();

         AvancarBtn.Click += AvancarBtn_Click;

         PreviewKeyUp += WRecuperarPassword_PreviewKeyUp;

         Clean();
      }

      #region Events

      private void AvancarBtn_Click(object sender, RoutedEventArgs e)
      {
         try
         {
            ProcessarPedido();
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private void WRecuperarPassword_PreviewKeyUp(object sender, KeyEventArgs e)
      {
         try
         {
            if (e.Key == Key.Enter)
            {
               ProcessarPedido();
            }
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
         AlterarModoEcra(RecuperarPasswordViewMode.EfetuarPedido);
         DataContext = _viewModel;
         //txtPassword.Password = null;

         txtEmail.Focus();
      }

      private void ProcessarPedido()
      {
         if (_viewModel.modoEcra == RecuperarPasswordViewMode.EfetuarPedido)
            EfetuarPedidoRecuperacao();
         else if (_viewModel.modoEcra == RecuperarPasswordViewMode.ConfirmarPedido)
            EfetuarConfirmacaoRecuperacao();
      }

      private async void EfetuarPedidoRecuperacao()
      {
         try
         {
            if (_viewModel.isLoading || _viewModel.modoEcra != RecuperarPasswordViewMode.EfetuarPedido)
               return;

            _viewModel.email = txtEmail.Text;

            if (string.IsNullOrEmpty(_viewModel.email))
            {
               Message.Warning("You need to fill in your email address.");
               return;
            }   

            _viewModel.NotifyValue(nameof(_viewModel.isLoading), true);

            var pedidoResponse = await WebApiCall.Users.ForgotPassword(AppWebClient.Instance.GetClient(), new ForgotPasswordRequest
            {
               email = _viewModel.email
            });

            if (pedidoResponse != null && pedidoResponse.Success)
            {
               AlterarModoEcra(RecuperarPasswordViewMode.ConfirmarPedido);
            }
            else
            {
               Message.Error("An error occurred. Please try again later.");
               return;
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

      private async void EfetuarConfirmacaoRecuperacao()
      {
         try
         {
            if (_viewModel.isLoading)
               return;

            _viewModel.novaPassword = txtPassword.Password;
            _viewModel.confirmacaoNovaPassword = txtConfirmPassword.Password;

            if (string.IsNullOrEmpty(_viewModel.codigo))
            {
               Message.Warning("You need to fill in the confirmation code.");
               return;
            }

            if (string.IsNullOrEmpty(_viewModel.novaPassword))
            {
               Message.Warning("You need to enter the new password.");
               return;
            }

            if (string.IsNullOrEmpty(_viewModel.confirmacaoNovaPassword))
            {
               Message.Warning("You need to fill in the new password confirmation field.");
               return;
            }

            if(_viewModel.novaPassword != _viewModel.confirmacaoNovaPassword)
            {
               Message.Warning("The new password and its confirmation do not match.");
               return;
            }

            _viewModel.NotifyValue(nameof(_viewModel.isLoading), true);


            var confirmacaoResponse = await WebApiCall.Users.ResetPassword(AppWebClient.Instance.GetClient(), new ResetPasswordRequest
            {
               token = _viewModel.codigo,
               newPassword = _viewModel.novaPassword
            });

            if (confirmacaoResponse != null && confirmacaoResponse.Success)
            {
               Message.Success("Your password has been successfully reset.", "Password Reset");

               this.Close();
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

      private void AlterarModoEcra(RecuperarPasswordViewMode modoEcra = RecuperarPasswordViewMode.EfetuarPedido)
      {
         _viewModel.modoEcra = modoEcra;

         if(_viewModel.modoEcra == RecuperarPasswordViewMode.EfetuarPedido)
         {
            Title = "Password Recovery Request";
            AvancarBtn.Content = "Request Recovery";
            grdPedidoRecuperacao.Visibility = Visibility.Visible;
            grdConfirmacaoRecuperacao.Visibility = Visibility.Collapsed;
         }
         else if(_viewModel.modoEcra == RecuperarPasswordViewMode.ConfirmarPedido)
         {
            Title = "Password Recovery Confirm";
            AvancarBtn.Content = "Confirm Recovery";
            grdPedidoRecuperacao.Visibility = Visibility.Collapsed;
            grdConfirmacaoRecuperacao.Visibility = Visibility.Visible;
         }
      }

      #endregion
   }
}
