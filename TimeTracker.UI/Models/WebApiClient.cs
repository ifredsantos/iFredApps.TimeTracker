using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TimeTracker.UI.Models
{
   public class WebApiClient
   {
      private HttpClient _httpclient;
      private string _baseAddress;

      public WebApiClient(string baseAddress)
      {
         _baseAddress = baseAddress;
      }

      #region Public methods

      public async Task<string> GetAsync(string endpointAddr, params object[] values)
      {
         string result = null;

         var client = createClient();

         string requestAddr = getRequestAddress(endpointAddr, values);
         var response = await client.GetAsync("https://localhost:7041/api/" + requestAddr);
         if (response.IsSuccessStatusCode)
         {
            result = await response.Content.ReadAsStringAsync();
            Console.WriteLine(result);
         }
         else
         {
            Console.WriteLine($"Erro: {response.StatusCode}");
         }

         return result;
      }

      #endregion

      #region Private methods


      private string getRequestAddress(string requestAddress, params object[] args)
      {
         return string.Format(requestAddress, args.ToArray());
      }

      #endregion

      #region Internal methods

      internal HttpClient createClient()
      {
         HttpClient client = null;

         try
         {
            client = new HttpClient();
            client.BaseAddress = new Uri(_baseAddress);
         }
         catch (Exception ex)
         {

            throw;
         }

         return client;
      }         

      #endregion
   }
}
