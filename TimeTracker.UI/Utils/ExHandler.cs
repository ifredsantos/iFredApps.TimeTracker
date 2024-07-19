using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TimeTracker.UI.Utils
{
   public static class ExHandler
   {
      public static void ShowException(this Exception ex)
      {
         string errMsg = "";

         errMsg += "Mensagem: ";
         errMsg += Environment.NewLine;
         errMsg += ex.Message;
         errMsg += Environment.NewLine;

         errMsg += "Stack Trace: ";
         errMsg += Environment.NewLine;
         errMsg += ex.StackTrace;

         MessageBox.Show(errMsg, "Calm down! An error has occurred!", MessageBoxButton.OK, MessageBoxImage.Error);
      }
   }
}
