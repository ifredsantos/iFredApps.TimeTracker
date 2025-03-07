using iFredApps.Lib.Wpf.Messages;
using System;
using System.Collections.Generic;
using System.Text;

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

            Message.Error(errorStr.ToString());

            return response.Data;
         }
      }
   }
}
