using iFredApps.Lib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace iFredApps.TimeTracker.UI.Models
{
   public class TimeManager : INotifyPropertyChanged
   {
      public TimeManagerTaskSession current_session { get; set; }
      public List<TimeManagerTaskSession> sessions { get; set; }
      public IFAObservableCollection<TimeManagerGroup> task_groups { get; set; }
      public List<Workspace> Workspaces { get; set; }
      public Workspace SelectedWorkspace { get; set; }
      public bool isLoading { get; set; }
      public TimeManager()
      {
         current_session = new TimeManagerTaskSession();
         sessions = new List<TimeManagerTaskSession>();
         task_groups = new IFAObservableCollection<TimeManagerGroup>();
      }

      public event PropertyChangedEventHandler PropertyChanged;
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
      public IFAObservableCollection<TimeManagerTask> tasks { get; set; }
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
      public IFAObservableCollection<TimeManagerTaskSession> sessions { get; set; }

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

      public bool is_detail_session_open { get; set; }

      public event PropertyChangedEventHandler PropertyChanged;
   }

   public class TimeManagerTaskSession : Session, INotifyPropertyChanged
   {
      public TimeSpan total_time { get; set; }
      public bool is_editing { get; set; }
      public bool is_working { get; set; }
      public bool is_detail_open { get; set; }

      public event PropertyChangedEventHandler PropertyChanged;
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
