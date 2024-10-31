using iFredApps.Lib.WebApi;
using System;
using System.Threading.Tasks;
using iFredApps.Lib.Wpf.Execption;

namespace iFredApps.TimeTracker.UI.Models
{
   public class AppWebClient : Singleton<AppWebClient>
   {
      private WebApiClient _client;

      private string _user = "";
      private string _password = "";
      private User _userData;

      public AppWebClient()
      {
      }

      public string Address { get; set; }
      public AppConfig appConfig { get; set; }


      public async Task Login(string user, string password)
      {
         try
         {
            _client = new WebApiClient(Address);

            _userData = await _client.PostAsync<User>("Users/Login", new LoginModel { UserSearchTerm = user, Password = password });
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      public User GetLoggedUserData()
      {
         return _userData;
      }

      public WebApiClient GetClient()
      {
         if (_client == null)
            throw new Exception("Client not initialized!");

         try
         {
            if (!IsClientValid())
            {
               Login(_user, _password).GetAwaiter();
            }
         }
         catch
         {
            _client = null;
         }

         return _client;
      }

      private bool IsClientValid()
      {
         return true;
      }

      private class LoginModel
      {
         public string UserSearchTerm { get; set; }
         public string Password { get; set; }
      }
   }
}
