using System;

namespace TimeTracker.UI.Models
{
    public class TimeRowChangedEventArgs : EventArgs
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
}
