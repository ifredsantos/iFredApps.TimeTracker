using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace TimeTracker.UI.Models
{
    public class TimeManagerTaskBase
    {
        public long id_task { get; set; }
        public string description { get; set; }
        public ObservableCollection<TimeManagerTaskSession> sessions { get; set; }

        public TimeManagerTaskBase()
        {
            sessions = new ObservableCollection<TimeManagerTaskSession>();
        }
    }

    public class TimeManagerTaskSessionBase
    {
        public long id_task { get; set; }
        public long id_session { get; set; }
        public DateTime start_date { get; set; }
        public DateTime? end_date { get; set; }
        public string description { get; set; }
        public string observation { get; set; }
    }
}
