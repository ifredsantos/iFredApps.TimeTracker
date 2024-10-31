using System;

namespace iFredApps.TimeTracker.UI.Models
{
   public class TimeRowSessionEventArgs : EventArgs
   {
      public TimeManagerTaskSession SessionData { get; set; }
   }

   public class TimeTaskContinueEventArgs : EventArgs
   {
      public TimeManagerTask TaskData { get; set; }
   }

   public class TimeTaskRemoveEventArgs : EventArgs
   {
      public TimeManagerTask TaskData { get; set; }
   }

   public class TimeTaskEditEventArgs : EventArgs
   {
      public string oldDescription { get; set; }
      public TimeManagerTask TaskData { get; set; }
   }

   public class TimeTaskSessionEditEventArgs : EventArgs
   {
      public TimeManagerTaskSession Session { get; set; }
   }
}
