using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using iFredApps.TimeTracker.UI.Models;
using iFredApps.Lib.Wpf.Execption;
using iFredApps.Lib;
using System.Threading.Tasks;

namespace iFredApps.TimeTracker.UI.Views
{
   /// <summary>
   /// Interaction logic for ucTimeManager.xaml
   /// </summary>
   public partial class ucTimeManagerView : UserControl
   {
      public event EventHandler<NotificationEventArgs> OnNotificationShow;

      private TimeManager m_timeManager = null;
      private bool _isFirstLoadComplete = false;

      public ucTimeManagerView()
      {
         InitializeComponent();

         m_timeManager = new TimeManager();

         DataContext = m_timeManager;

         Loaded += UcTimeManagerView_Loaded;
      }

      #region Private Methods

      private async Task InitData()
      {
         try
         {
            m_timeManager.NotifyValue(nameof(m_timeManager.isLoading), true);

            var data = await DatabaseManager.LoadData();
            if (data != null)
            {
               if (data.sessions != null && data.sessions.Count > 0)
               {
                  foreach (var session in data.sessions)
                  {
                     m_timeManager.sessions.Add(session);
                  }
               }

               if (data.uncompleted_session != null)
               {
                  if (MessageBox.Show(string.Format("There is one session to complete \"{0}\". Do you want to resume?", data.uncompleted_session.description), "Calm down!",
                          MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes
                      ) == MessageBoxResult.Yes)
                  {
                     m_timeManager.current_session = new TimeManagerTaskSession
                     {
                        session_id = data.uncompleted_session.session_id,
                        user_id = data.uncompleted_session.user_id,
                        description = data.uncompleted_session.description,
                        start_date = data.uncompleted_session.start_date,
                        observation = data.uncompleted_session.observation,
                        total_time = DateTime.Now - data.uncompleted_session.start_date
                     };
                     m_timeManager.NotifyValue(nameof(m_timeManager.current_session));

                     timeRowEditor.StartStopSession();
                  }
                  else
                  {
                     m_timeManager.current_session = new TimeManagerTaskSession();

                     await DatabaseManager.DeleteSession(data.uncompleted_session.session_id, OnNotificationShow);
                  }
               }
            }

            await GroupingSessionIntoTasks();

            _isFirstLoadComplete = true;
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
         finally
         {
            m_timeManager.NotifyValue(nameof(m_timeManager.isLoading), false);
         }
      }

      private async Task GroupingSessionIntoTasks()
      {
         m_timeManager.task_groups.Clear();

         if (m_timeManager.sessions != null)
         {
            Dictionary<DateTime, List<TimeManagerTask>> dicTasksByDate = new Dictionary<DateTime, List<TimeManagerTask>>();

            //Default Group
            dicTasksByDate.Add(DateTime.Now.Date, new List<TimeManagerTask>());

            foreach (var session in m_timeManager.sessions)
            {
               if (session.end_date.HasValue)
               {
                  DateTime dateReference = session.end_date.Value.Date;

                  if (!dicTasksByDate.ContainsKey(dateReference))
                  {
                     dicTasksByDate.Add(dateReference, new List<TimeManagerTask>()
                     {
                         new TimeManagerTask() {
                             description = session.description,
                             sessions = new IFAObservableCollection<TimeManagerTaskSession> { session },
                         }
                     });
                  }
                  else
                  {
                     if (dicTasksByDate.TryGetValue(dateReference, out List<TimeManagerTask> tasks))
                     {
                        bool addToTask = false;
                        if (tasks != null && tasks.Count > 0)
                        {
                           foreach (var task in tasks)
                           {
                              if (task.description == session.description)
                              {
                                 task.sessions.Add(session);
                                 addToTask = true;
                              }
                           }
                        }

                        if (!addToTask)
                        {
                           tasks.Add(new TimeManagerTask()
                           {
                              description = session.description,
                              sessions = new IFAObservableCollection<TimeManagerTaskSession> { session },
                           });
                        }
                     }
                  }
               }
            }

            List<TimeManagerGroup> lstGroups = new List<TimeManagerGroup>();
            foreach (var dicRow in dicTasksByDate.OrderByDescending(x => x.Key))
            {
               TimeManagerGroup group = new TimeManagerGroup
               {
                  date_group_reference = dicRow.Key,
                  tasks = new IFAObservableCollection<TimeManagerTask>(),
               };
               foreach (var task in dicRow.Value)
               {
                  group.tasks.Add(task);
               }
               lstGroups.Add(group);
            }

            await Task.Delay(10);
            m_timeManager.task_groups.AddRange(lstGroups);
         }
      }

      #endregion

      #region Events

      private async void UcTimeManagerView_Loaded(object sender, RoutedEventArgs e)
      {
         try
         {
            if (!_isFirstLoadComplete)
            {
               await InitData();
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private async void OnCurrentSessionChanged(object sender, TimeRowSessionEventArgs e)
      {
         try
         {
            await DatabaseManager.UpdateSession(e.SessionData, OnNotificationShow);

            m_timeManager.sessions.Add(e.SessionData);

            m_timeManager.NotifyValue(nameof(m_timeManager.current_session), new TimeManagerTaskSession());

            await GroupingSessionIntoTasks();
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private void OnTaskContinueClick(object sender, TimeTaskContinueEventArgs e)
      {
         try
         {
            if (e.TaskData != null)
            {
               if (m_timeManager.current_session != null && m_timeManager.current_session.is_working)
               {
                  MessageBox.Show("There is already a session in progress. Please stop the current session and try again.", "Calm down!", MessageBoxButton.OK, MessageBoxImage.Warning);
                  return;
               }
               else
               {
                  m_timeManager.NotifyValue(nameof(m_timeManager.current_session), new TimeManagerTaskSession
                  {
                     description = e.TaskData.description,
                  });
               }
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private async void OnTaskRemoveClick(object sender, TimeTaskRemoveEventArgs e)
      {
         try
         {
            if (e.TaskData != null)
            {
               if (MessageBox.Show(string.Format("Are you sure you want to remove the \"{0}\" task?", e.TaskData.description), "Calm down!",
                       MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes
                   ) != MessageBoxResult.Yes)
               {
                  return;
               }

               if (e.TaskData.sessions != null && e.TaskData.sessions.Count > 0)
               {
                  foreach (var session in e.TaskData.sessions)
                  {
                     await DatabaseManager.DeleteSession(session.session_id, OnNotificationShow);
                     m_timeManager.sessions.Remove(session);
                  }
               }

               await GroupingSessionIntoTasks();
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private async void OnTaskChange(object sender, TimeTaskEditEventArgs e)
      {
         try
         {
            if (e.TaskData != null && e.TaskData.sessions != null)
            {
               foreach (var session in e.TaskData.sessions)
               {
                  session.description = e.TaskData.description;
                  await DatabaseManager.UpdateSession(session, OnNotificationShow);
               }

               await GroupingSessionIntoTasks();
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private async void OnSessionChange(object sender, TimeTaskSessionEditEventArgs e)
      {
         try
         {
            if (e.Session != null)
            {
               await DatabaseManager.UpdateSession(e.Session, OnNotificationShow);

               await GroupingSessionIntoTasks();
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private async void OnSessionRemove(object sender, TimeTaskSessionEditEventArgs e)
      {
         try
         {
            if (e.Session != null)
            {
               await DatabaseManager.DeleteSession(e.Session.session_id, OnNotificationShow);
               m_timeManager.sessions.Remove(e.Session);

               await GroupingSessionIntoTasks();
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private async void SessionStarts(object sender, TimeRowSessionEventArgs e)
      {
         if (m_timeManager.current_session.session_id == 0) //only record if it is a new session (not a recovered session)
         {
            var sessionData = await DatabaseManager.CreateSession(m_timeManager.current_session, OnNotificationShow);
            if (sessionData != null)
            {
               m_timeManager.current_session.session_id = sessionData.session_id;
               m_timeManager.current_session.user_id = sessionData.user_id;
            }
         }
      }

      #endregion
   }
}
