using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TimeTracker.UI.Models
{
   public class TimeManager : INotifyPropertyChanged
   {
      private TimeManagerTaskCurrentSession _current_session;
      [JsonIgnore]
      public TimeManagerTaskCurrentSession current_session
      {
         get => _current_session;
         set
         {
            if (value == _current_session) return;
            _current_session = value;
            NotifyPropertyChanged();
         }
      }
      public List<TimeManagerTaskSession> sessions { get; set; }
      [JsonIgnore]
      public ObservableCollection<TimeManagerGroup> task_groups { get; set; }
      public TimeManager()
      {
         current_session = new TimeManagerTaskCurrentSession();
         sessions = new List<TimeManagerTaskSession>();
         task_groups = new ObservableCollection<TimeManagerGroup>();
      }

      public event PropertyChangedEventHandler PropertyChanged;
      protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
      {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }
   }

   public class TimeManagerGroup
   {
      public string description
      {
         get
         {
            if (date_group_reference != DateTime.MinValue)
            {
               if (date_group_reference.Date == DateTime.Now.Date)
               {
                  return "Today";
               }
               else
               {
                  return date_group_reference.ToString("dd/MM/yyyy");
               }
            }

            return null;
         }
      }
      public TimeSpan total_time { get; set; }
      public ObservableCollection<TimeManagerTask> tasks { get; set; }
      public DateTime date_group_reference { get; set; }
      public TimeSpan tasks_total_time
      {
         get
         {
            TimeSpan result = TimeSpan.Zero;

            if (tasks != null)
            {
               foreach (var task in tasks)
               {
                  result += task.session_total_time;
               }
            }

            return result;
         }
      }
   }

   public class TimeManagerTask : INotifyPropertyChanged
   {
      public string description { get; set; }
      public ObservableCollection<TimeManagerTaskSession> sessions { get; set; }

      public TimeSpan session_total_time
      {
         get
         {
            TimeSpan result = TimeSpan.Zero;

            if (sessions != null)
            {
               foreach (var session in sessions)
               {
                  if (session.end_date.HasValue)
                  {
                     result += (session.end_date - session.start_date).Value;
                  }
               }
            }

            return result;
         }
      }
      private bool _is_detail_session_open;
      public bool is_detail_session_open
      {
         get => _is_detail_session_open;
         set
         {
            if (value == _is_detail_session_open) return;
            _is_detail_session_open = value;
            NotifyPropertyChanged();
         }
      }

      public event PropertyChangedEventHandler PropertyChanged;
      protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
      {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }
   }

   public class TimeManagerTaskSession : Session, INotifyPropertyChanged
   {
      private TimeSpan _total_time;
      [JsonIgnore]
      public TimeSpan total_time
      {
         get
         {
            if (_total_time == TimeSpan.Zero && end_date.HasValue)
               _total_time = end_date.Value - start_date;
            return _total_time;
         }
         set
         {
            if (value == _total_time) return;
            _total_time = value;
            NotifyPropertyChanged();
         }
      }

      public event PropertyChangedEventHandler PropertyChanged;
      protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
      {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }
   }

   public class TimeManagerTaskCurrentSession : TimeManagerTaskSession, INotifyPropertyChanged
   {
      private bool _is_working;
      [JsonIgnore]
      public bool is_working
      {
         get => _is_working;
         set
         {
            if (value == _is_working) return;
            _is_working = value;
            NotifyPropertyChanged();
         }
      }
   }

   public class TimeManagerDatabaseData
   {
      public TimeManagerTaskSession uncompleted_session { get; set; }
      public List<TimeManagerTaskSession> sessions { get; set; }
      public TimeManagerDatabaseData()
      {
         sessions = new List<TimeManagerTaskSession>();
      }
   }
}
