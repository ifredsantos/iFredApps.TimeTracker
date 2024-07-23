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
                m_timeManager.tasks.Clear();

                AppConfig appConfig = ((App)Application.Current)?.Config;

                if (appConfig != null && appConfig.database_type == AppConfig.enDataBaseType.JSON && appConfig.json_database_config != null)
                {
                    string directory = appConfig.json_database_config.directory;
                    string filename = appConfig.json_database_config.filename;
                    string databaseFileDir = Path.Combine(directory, filename);

                    if (File.Exists(databaseFileDir))
                    {
                        string tasksJSON = File.ReadAllText(databaseFileDir);
                        List<TimeManagerTask> dbTasks = JsonConvert.DeserializeObject<List<TimeManagerTask>>(tasksJSON);
                        if (dbTasks != null && dbTasks.Count > 0)
                        {
                            dbTasks.ForEach(x => m_timeManager.tasks.Add(x));
                        }
                    }
                }

                RefreshTasks();
            }
            catch (Exception ex)
            {
                ex.ShowException();
            }
        }

        private void RefreshTasks()
        {
            m_timeManager.task_groups.Clear();

            if (m_timeManager.tasks != null)
            {
                Dictionary<DateTime, List<TimeManagerTask>> dicTasksByDate = new Dictionary<DateTime, List<TimeManagerTask>>();

                //Default Group
                dicTasksByDate.Add(DateTime.Now.Date, new List<TimeManagerTask>());

                foreach (var task in m_timeManager.tasks)
                {
                    if (task.sessions != null && task.sessions.Count > 0)
                    {
                        foreach (var session in task.sessions)
                        {
                            if (session.end_date.HasValue)
                            {
                                DateTime dateReference = session.end_date.Value;

                                if (!dicTasksByDate.ContainsKey(dateReference))
                                {
                                    dicTasksByDate.Add(dateReference, new List<TimeManagerTask>() { task });
                                }
                                else
                                {
                                    if (dicTasksByDate.TryGetValue(dateReference, out List<TimeManagerTask> tasks))
                                    {
                                        tasks.Add(task);
                                    }
                                }
                            }
                            else
                            {
                                //TODO: Group for unsaved data
                            }
                        }
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

                    string tasksJSON = JsonConvert.SerializeObject(m_timeManager.tasks);
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
                TimeManagerTask existingTask = m_timeManager.tasks.ToList().Find(x => x.description == e.SessionData.description);

                if (existingTask != null) //If a task exists, it must be updated
                {
                    long maxSessionID = existingTask.sessions.Max(x => x.id_session);

                    e.SessionData.id_task = existingTask.id_task;
                    e.SessionData.id_session = maxSessionID + 1;

                    existingTask.sessions.Add(e.SessionData);
                }
                else //If a task does not exist, it must be created
                {
                    long maxTaskID = 0;

                    if (m_timeManager.tasks != null && m_timeManager.tasks.Count > 0)
                        maxTaskID = m_timeManager.tasks.Max(x => x.id_task);

                    long newTaskID = maxTaskID + 1;

                    e.SessionData.id_task = newTaskID;

                    TimeManagerTask newTask = new TimeManagerTask
                    {
                        id_task = newTaskID,
                        description = e.SessionData.description,
                        sessions = new ObservableCollection<TimeManagerTaskSession>
                        {
                           e.SessionData
                        }
                    };

                    m_timeManager.tasks.Add(newTask);
                }

                m_timeManager.current_session = new TimeManagerTaskCurrentSession();
                RefreshTasks();

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
                            id_task = e.TaskData.id_task,
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

                    m_timeManager.tasks.Remove(e.TaskData);

                    RefreshTasks();
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
                    var originalTask = m_timeManager.tasks.FirstOrDefault(x => x.id_task == e.TaskData.id_task);
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
                    SaveTasks();
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
