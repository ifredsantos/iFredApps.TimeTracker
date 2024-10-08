using System;

namespace TimeTracker.UI.Models
{
   public class NotificationEventArgs : EventArgs
   {
      public string Message { get; set; }
      public TimeSpan? Duration { get; set; }
      public NotificationEventArgs(string message)
      {
         Message = message;
      }

      public NotificationEventArgs(string message, TimeSpan duration)
      {
         Message = message;
         Duration = duration;
      }

      public NotificationEventArgs(string message, double durationInSeconds)
      {
         Message = message;
         Duration = TimeSpan.FromSeconds(durationInSeconds);
      }
   }
}
