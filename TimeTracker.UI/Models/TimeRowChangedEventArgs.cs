using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTracker.UI.Models
{
   public class TimeRowChangedEventArgs : EventArgs
   {
      public TimeManagerTaskSession SessionData { get; set; }
   }
}
