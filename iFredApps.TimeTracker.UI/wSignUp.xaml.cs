using System;
using iFredApps.Lib.Wpf.Execption;
using iFredApps.Lib.Wpf.Messages;
using iFredApps.Lib;
using iFredApps.TimeTracker.UI.Models;
using iFredApps.TimeTracker.UI.Utils;
using MahApps.Metro.Controls;
using TimeTracker.SL;
using System.Windows.Documents;
using System.Collections.Generic;

namespace iFredApps.TimeTracker.UI
{
   /// <summary>
   /// Interaction logic for wSignUp.xaml
   /// </summary>
   public partial class wSignUp : MetroWindow
   {
      private AppConfig _appConfig;
      private SignUpViewModel _viewModel;

      public wSignUp()
      {
         InitializeComponent();

         SignUpBtn.Click += SignUpBtn_Click;

         _appConfig = SettingsLoader<AppConfig>.Instance.Data;

         Clean();
      }

      #region Events

      private void SignUpBtn_Click(object sender, System.Windows.RoutedEventArgs e)
      {
         try
         {
            SignUpSubmit();
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      #endregion

      #region Private Methods

      public void Clean()
      {
         _viewModel = new SignUpViewModel();
         DataContext = _viewModel;
         txtPassword.Password = null;
         txtConfirmPassword.Password = null;

         txtName.Focus();
      }

      private async void SignUpSubmit()
      {
         try
         {
            if (_viewModel.isLoading)
               return;

            _viewModel.password = txtPassword.Password;
            _viewModel.confirm_password = txtConfirmPassword.Password;

            _viewModel.NotifyValue(nameof(_viewModel.isLoading), true);

            List<string> warnings = ValidateData();
            if (!warnings.IsNullOrEmpty()) 
            {
               string allWarnings = "";
               foreach (var warning in warnings)
               {
                  allWarnings += warning + Environment.NewLine;
               }

               _viewModel.NotifyValue(nameof(_viewModel.isLoading), false);

               Message.Warning(allWarnings);

               return;
            }

            AppWebClient.Instance.Address = SettingsLoader<AppConfig>.Instance.Data?.webapi_connection_config?.baseaddress;

            sUserSignUp signUpData = new sUserSignUp
            {
               name = _viewModel.name,
               username = _viewModel.username,
               email = _viewModel.email,
               password = txtPassword.Password,
               confirm_password = txtConfirmPassword.Password,
            };
            var signUpResponse = await WebApiCall.Users.SignUp(AppWebClient.Instance.GetClient(), signUpData);

            if(signUpResponse.Success)
            {
               Message.Success("Account created successfully!");
               DialogResult = true;
               Close();
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

      private List<string> ValidateData()
      {
         List<string> result = new List<string>();

         if (string.IsNullOrEmpty(_viewModel.name))
         {
            result.Add("Fill in the name.");
         }

         if (string.IsNullOrEmpty(_viewModel.username))
         {
            result.Add("Fill in the username.");
         }

         if (string.IsNullOrEmpty(_viewModel.email))
         {
            result.Add("Fill in the email.");
         }

         if (string.IsNullOrEmpty(_viewModel.password))
         {
            result.Add("Fill in the password.");
         }

         if (_viewModel.confirm_password != _viewModel.password)
         {
            result.Add("Passwords must match.");
         }

         return result;
      }

      #endregion
   }
}
