using System;
using System.Collections.ObjectModel;

namespace TimeTracker.UI.Models
{
    public class TimeManagerTaskBase
    {
        public long id_task { get; set; }
        public string description { get; set; }
        public ObservableCollection<TimeManagerTaskSessionBase> sessions { get; set; }

        public TimeManagerTaskBase()
        {
            sessions = new ObservableCollection<TimeManagerTaskSessionBase>();
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
