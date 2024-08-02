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

        private TimeManager m_timeManager = null;

        public ucTimeManager()
        {
            InitializeComponent();
            Loaded += UcTimeManager_Loaded;
        }

        private void UcTimeManager_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                InitData();
            }
            catch (Exception ex)
            {
                ex.ShowException();
            }
        }

        private async void InitData()
        {
            try
            {
                m_timeManager = new TimeManager();

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
                            m_timeManager.current_session = new TimeManagerTaskCurrentSession
                            {
                                description = data.uncompleted_session.description,
                                start_date = data.uncompleted_session.start_date,
                                observation = data.uncompleted_session.observation,
                                total_time = DateTime.Now - data.uncompleted_session.start_date
                            };
                        }
                        else
                        {
                            m_timeManager.current_session = new TimeManagerTaskCurrentSession();

                            await DatabaseManager.SaveAllSessions(PrepareDataForRecording(), OnNotificationShow);
                        }
                    }
                }

                GroupingSessionIntoTasks();
            }
            catch (Exception ex)
            {
                ex.ShowException();
            }
            finally
            {
                DataContext = m_timeManager;
                
                if (!string.IsNullOrEmpty(m_timeManager.current_session.description))
                {
                    timeRowEditor.StartSession();
                }
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

        private TimeManagerDatabaseData PrepareDataForRecording()
        {
            TimeManagerDatabaseData result = new TimeManagerDatabaseData();
            result.sessions = m_timeManager.sessions;

            if (m_timeManager.current_session?.is_working != null && m_timeManager.current_session.is_working)
            {
                result.uncompleted_session = m_timeManager.current_session;
            }

            return result;
        }

        #region Events

        private async void OnCurrentSessionChanged(object sender, TimeRowSessionEventArgs e)
        {
            try
            {
                int maxSessionID = 0;

                if (m_timeManager?.sessions != null && m_timeManager.sessions.Count > 0)
                    maxSessionID = m_timeManager.sessions.Max(x => x.session_id);

                e.SessionData.session_id = maxSessionID + 1;

                m_timeManager.sessions.Add(e.SessionData);

                m_timeManager.current_session = new TimeManagerTaskCurrentSession();

                GroupingSessionIntoTasks();

                await DatabaseManager.SaveAllSessions(PrepareDataForRecording(), OnNotificationShow);
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
                            m_timeManager.sessions.Remove(session);
                        }
                    }

                    GroupingSessionIntoTasks();
                    await DatabaseManager.SaveAllSessions(PrepareDataForRecording(), OnNotificationShow);
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
                if (e.TaskData != null)
                {
                    var sessionsToChange = m_timeManager.sessions.FindAll(x => x.description == e.oldDescription);
                    if (sessionsToChange != null)
                    {
                        foreach (var session in sessionsToChange)
                        {
                            session.description = e.TaskData.description;
                        }
                    }

                    GroupingSessionIntoTasks();
                    await DatabaseManager.SaveAllSessions(PrepareDataForRecording(), OnNotificationShow);
                }
            }
            catch (Exception ex)
            {
                ex.ShowException();
            }
        }

        private async void SessionStarts(object sender, TimeRowSessionEventArgs e)
        {
            await DatabaseManager.SaveAllSessions(PrepareDataForRecording(), OnNotificationShow);
        }

        #endregion
    }
}
