using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeTracker.UI.Models
{
   public class TimeManager
   {
      public TimeManagerTask current_task { get; set; }
      public List<TimeManagerTask> tasks { get; set; }
      public List<TimeManagerGroup> task_groups { get; set; }
      public TimeManager()
      {
         tasks = new List<TimeManagerTask>();
         task_groups = new List<TimeManagerGroup>();
      }

      public void LoadTasks()
      {
         tasks.Clear();
         task_groups.Clear();

         #region Dummy data

         tasks.Add(new TimeManagerTask
         {
            id_task = 1,
            description = "Enterprise Website X",
            sessions = new List<TimeManagerTaskSession>
            {
               new TimeManagerTaskSession { id_task = 1, id_session = 1, start_date = new DateTime(2024, 7, 15, 9, 15, 0), end_date = new DateTime(2024, 7, 15, 11, 15, 0) },
               new TimeManagerTaskSession { id_task = 1, id_session = 2, start_date = new DateTime(2024, 7, 16, 9, 12, 0), end_date = new DateTime(2024, 7, 16, 11, 5, 0) },
               new TimeManagerTaskSession { id_task = 1, id_session = 3, start_date = new DateTime(2024, 7, 16, 11, 25, 0), end_date = new DateTime(2024, 7, 16, 13, 32, 0) },
            }
         });

         tasks.Add(new TimeManagerTask
         {
            id_task = 2,
            description = "CRM - enterprise x",
            sessions = new List<TimeManagerTaskSession>
            {
               new TimeManagerTaskSession { id_task = 2, id_session = 1, start_date = new DateTime(2024, 7, 17, 9, 47, 0), end_date = new DateTime(2024, 7, 17, 11, 2, 0) },
            }
         });

         tasks.Add(new TimeManagerTask
         {
            id_task = 2,
            description = "CRM - enterprise y",
            sessions = new List<TimeManagerTaskSession>
            {
               new TimeManagerTaskSession { id_task = 2, id_session = 1, start_date = new DateTime(2024, 7, 17, 12, 0, 0), end_date = new DateTime(2024, 7, 17, 13, 0, 0) },
            }
         });

         tasks.Add(new TimeManagerTask
         {
            id_task = 2,
            description = "ERP",
            sessions = new List<TimeManagerTaskSession>
            {
               new TimeManagerTaskSession { id_task = 3, id_session = 1, start_date = new DateTime(2024, 7, 13, 9, 47, 0), end_date = new DateTime(2024, 7, 13, 11, 2, 0) },
            }
         });

         #endregion

         if (tasks != null)
         {
            Dictionary<string, List<TimeManagerTask>> dicTasksByGroupName = new Dictionary<string, List<TimeManagerTask>>();
            foreach (var task in tasks)
            {
               TimeManagerTaskSession lastSession = task.sessions?.OrderByDescending(x => x.end_date)?.FirstOrDefault();
               if (lastSession != null)
               {
                  string groupKey = "";
                  if (lastSession.end_date.HasValue)
                  {
                     if (lastSession.end_date.Value.Date == DateTime.Now.Date)
                        groupKey = "Today";
                     else
                        groupKey = lastSession.end_date.Value.ToString("dd/MM/yyyy");
                  }
                  else
                  {
                     groupKey = "Unrecorded data";
                  }

                  if (dicTasksByGroupName.ContainsKey(groupKey))
                  {
                     List<TimeManagerTask> taskList = null;
                     if (dicTasksByGroupName.TryGetValue(groupKey, out taskList))
                     {
                        taskList.Add(task);
                     }
                  }
                  else
                  {
                     dicTasksByGroupName.Add(groupKey, new List<TimeManagerTask> { task });
                  }
               }
            }

            foreach (var dicRow in dicTasksByGroupName.OrderByDescending(x => x.Key))
            {
               List<TimeManagerTask> taskList = dicRow.Value;
               TimeSpan totalTime = TimeSpan.Zero;
               if (taskList != null)
               {
                  //TODO: Calc total time
                  //totalTime = taskList.SelectMany(x => x.sessions.Where(l => l.end_date.HasValue)).Sum(x => x.end_date - x.start_date);
               }
               task_groups.Add(new TimeManagerGroup { description = dicRow.Key, tasks = taskList, total_time = TimeSpan.Zero });
            }
         }
      }
   }

   public class TimeManagerGroup
   {
      public string description { get; set; }
      public TimeSpan total_time { get; set; }
      public List<TimeManagerTask> tasks { get; set; }
   }

   public class TimeManagerTask
   {
      public long id_task { get; set; }
      public string description { get; set; }
      public List<TimeManagerTaskSession> sessions { get; set; }

      public TimeManagerTask()
      {
         sessions = new List<TimeManagerTaskSession>();
      }
   }

   public class TimeManagerTaskSession
   {
      public long id_task { get; set; }
      public long id_session { get; set; }
      public DateTime start_date { get; set; }
      public DateTime? end_date { get; set; }
      public string observation { get; set; }
   }
}
