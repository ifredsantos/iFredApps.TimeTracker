using iFredApps.Lib.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTracker.UI.Models
{
   public class AppWebClient : Singleton<AppWebClient>
   {
      private string _apiBaseAddress;
      private WebApiClient _apiClient;

      private string _user;
      private string _password;

      public AppWebClient()
      {

      }

      public AppConfig appConfig { get; set; }

   
      public void Login(string user, string password)
      {
         _apiClient = new WebApiClient(_apiBaseAddress);

      }


      public WebApiClient GetClient()
      {
         if (_apiClient == null)
            throw new Exception("Client not initialized!");

         try
         {
            if (!IsClientValid())
            {
               Login(_user, _password);
            }
         }
         catch
         {
            _apiClient = null;
         }

         return _apiClient;
      }

      private bool IsClientValid()
      {
         return true;
      }
   }
}
