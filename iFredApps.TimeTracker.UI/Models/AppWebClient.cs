using iFredApps.Lib.WebApi;
using iFredApps.Lib.Wpf.Execption;
using System;
using System.Threading.Tasks;
using TimeTracker.SL;

namespace iFredApps.TimeTracker.UI.Models
{
   public class AppWebClient : Singleton<AppWebClient>
   {
      private WebApiClient _client;

      private string _user = "";
      private string _password = "";
      private sUser _userData;

      public AppWebClient()
      {
      }

      public string Address { get; set; }
      public AppConfig appConfig { get; set; }


      public void Init(string baseAddress)
      {
         Address = baseAddress;
         _client = new WebApiClient(Address);
      }

      public async Task Login(string user, string password)
      {
         try
         {
            var loginResponse = await WebApiCall.Users.Login(_client, user, password);
            _userData = loginResponse?.TrataResposta();
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      public void Logout()
      {
         _userData = null;
         _user = null;
         _password = null;
      }

      public sUser GetLoggedUserData()
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
   }
}
