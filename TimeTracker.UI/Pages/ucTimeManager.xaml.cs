using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TimeTracker.UI.Models;
using TimeTracker.UI.Utils;

namespace TimeTracker.UI.Pages
{
   /// <summary>
   /// Interaction logic for ucTimeManager.xaml
   /// </summary>
   public partial class ucTimeManager : UserControl
   {
      public event EventHandler<NotificationEventArgs> OnNotificationShow;

      private TimeManager m_timeManager = new TimeManager();

      public ucTimeManager()
      {
         InitializeComponent();

         LoadTasks();

         DataContext = m_timeManager;

         lstView.ItemsSource = m_timeManager.task_groups;
      }

      private void LoadTasks()
      {
         try
         {
            AppConfig appConfig = ((App)Application.Current)?.Config;

            if (appConfig != null && appConfig.database_type == AppConfig.enDataBaseType.JSON && appConfig.json_database_config != null)
            {
               string directory = appConfig.json_database_config.directory;
               string filename = appConfig.json_database_config.filename;
               string databaseFileDir = Path.Combine(directory, filename);

               if (File.Exists(databaseFileDir))
               {
                  string tasksJSON = File.ReadAllText(databaseFileDir);
                  m_timeManager.sessions = JsonConvert.DeserializeObject<List<TimeManagerTaskSession>>(tasksJSON);
               }
            }

            GroupingSessionIntoTasks();
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private void GroupingSessionIntoTasks()
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
                                    sessions = new ObservableCollection<TimeManagerTaskSession> { session },
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
                              sessions = new ObservableCollection<TimeManagerTaskSession> { session },
                           });
                        }
                     }
                  }
               }
               else
               {
                  //TODO: Group for unsaved data
               }
            }

            foreach (var dicRow in dicTasksByDate.OrderByDescending(x => x.Key))
            {
               TimeManagerGroup group = new TimeManagerGroup
               {
                  date_group_reference = dicRow.Key,
                  tasks = new ObservableCollection<TimeManagerTask>(),
               };
               foreach (var task in dicRow.Value)
               {
                  group.tasks.Add(task);
               }
               m_timeManager.task_groups.Add(group);
            }
         }
      }

      private void SaveTasks()
      {
         Task.Factory.StartNew(() =>
         {
            try
            {
               AppConfig appConfig = ((App)Application.Current).Config;

               string directory = appConfig.json_database_config.directory;
               string filename = appConfig.json_database_config.filename;
               string databaseFileDir = Path.Combine(directory, filename);

               if (!Directory.Exists(directory))
                  Directory.CreateDirectory(directory);

               string tasksJSON = JsonConvert.SerializeObject(m_timeManager.sessions);
               File.WriteAllText(databaseFileDir, tasksJSON);
            }
            catch (Exception ex)
            {
               ex.ShowException();
            }
         })
         .ContinueWith(t =>
         {
            OnNotificationShow?.Invoke(this, new NotificationEventArgs("Data synchronized successfully!", 3));
         }, TaskScheduler.FromCurrentSynchronizationContext());
      }

      #region Events

      private void OnCurrentSessionChanged(object sender, TimeRowChangedEventArgs e)
      {
         try
         {
            long maxSessionID = 0;

            if (m_timeManager?.sessions != null && m_timeManager.sessions.Count > 0)
               maxSessionID = m_timeManager.sessions.Max(x => x.id_session);

            e.SessionData.id_session = maxSessionID + 1;

            m_timeManager.sessions.Add(e.SessionData);

            m_timeManager.current_session = new TimeManagerTaskCurrentSession();

            GroupingSessionIntoTasks();

            SaveTasks();
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
                  m_timeManager.current_session = new TimeManagerTaskCurrentSession
                  {
                     description = e.TaskData.description,
                  };
               }
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private void OnTaskRemoveClick(object sender, TimeTaskRemoveEventArgs e)
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
                     m_timeManager.sessions.Remove(session);
                  }
               }

               GroupingSessionIntoTasks();
               SaveTasks();
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private void OnTaskChange(object sender, TimeTaskRemoveEventArgs e)
      {
         try
         {
            if (e.TaskData != null)
            {
               /*var originalTask = m_timeManager.tasks.FirstOrDefault(x => x.id_task == e.TaskData.id_task);
               if (originalTask != null)
               {
                   originalTask.description = e.TaskData.description;

                   if (originalTask.sessions != null)
                   {
                       foreach (var session in originalTask.sessions)
                       {
                           session.description = e.TaskData.description;
                       }
                   }
               }

               RefreshTasks();
               SaveTasks(); */
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      #endregion
   }
}
