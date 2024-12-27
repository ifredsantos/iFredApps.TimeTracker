using iFredApps.Lib;
using iFredApps.Lib.Wpf.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFredApps.TimeTracker.UI.Models
{
   public class ApiResponse<T>
   {
      public bool Success { get; set; } = false;
      public T Data { get; set; }
      public List<string> Errors { get; set; } = new List<string>();
   }

   public static class ApiResponseExtensions
   {
      public static T TrataResposta<T>(this ApiResponse<T> response)
      {
         if (response == null)
            throw new ArgumentNullException(nameof(response), "The answer cannot be null.");

         if (response.Success)
         {
            return response.Data;
         }
         else
         {
            // Constrói a string de erros caso existam
            var errorStr = new StringBuilder();
            if (response.Errors != null && response.Errors.Count > 0)
            {
               foreach (var error in response.Errors)
               {
                  errorStr.AppendLine(error);
               }
            }
            else
            {
               errorStr.AppendLine("An unknown error has occurred.");
            }

            // Exibe a mensagem de erro para o usuário (presumindo que Message.Error é válido)
            Message.Error(errorStr.ToString());

            // Retorna o dado, mesmo em caso de falha, conforme o comportamento original
            return response.Data;
         }
      }
   }
}
