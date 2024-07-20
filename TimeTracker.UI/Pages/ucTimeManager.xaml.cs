using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TimeTracker.UI.Models;
using TimeTracker.UI.Utils;

namespace TimeTracker.UI.Pages
{
    /// <summary>
    /// Interaction logic for ucTimeManager.xaml
    /// </summary>
    public partial class ucTimeManager : UserControl
    {
        #region Variables

        private TimeManager m_timeManager = new TimeManager();

        #endregion

        public ucTimeManager()
        {
            InitializeComponent();

            LoadStartInfo();

            DataContext = m_timeManager;

            lstView.ItemsSource = m_timeManager.task_groups;
        }

        #region Private Methods

        private void LoadStartInfo()
        {
            try
            {
                m_timeManager.LoadTasks();
            }
            catch (Exception ex)
            {
                ex.ShowException();
            }
        }

        #endregion

        #region Events

        private void OnSessionRowChanged(object sender, TimeRowChangedEventArgs e)
        {
            try
            {
                TimeManagerTask existingTask = m_timeManager.tasks.ToList().Find(x => x.description == e.SessionData.description);
                if (existingTask != null)
                {
                    long maxSessionID = existingTask.sessions.Max(x => x.id_session);

                    e.SessionData.id_task = existingTask.id_task;
                    e.SessionData.id_session = maxSessionID + 1;

                    existingTask.sessions.Add(e.SessionData);
                }
                else
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
                m_timeManager.RefreshTasks();

                SaveTasks();
            }
            catch (Exception ex)
            {
                ex.ShowException();
            }
        }

        private void OnTaskContinueClick(object sender, TimeTaskContinueEventArgs e)
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

        private void OnTaskRemoveClick(object sender, TimeTaskRemoveEventArgs e)
        {
            if (e.TaskData != null)
            {
                m_timeManager.tasks.Remove(e.TaskData);

                m_timeManager.RefreshTasks();
                SaveTasks();
            }
        }

        private void OnTaskChange(object sender, TimeTaskRemoveEventArgs e)
        {
            if (e.TaskData != null)
            {
                var originalTask = m_timeManager.tasks.FirstOrDefault(x => x.id_task == e.TaskData.id_task);
                if(originalTask != null)
                {
                    originalTask.description = e.TaskData.description;

                    if(originalTask.sessions != null)
                    {
                        foreach (var session in originalTask.sessions)
                        {
                            session.description = e.TaskData.description;
                        }
                    }
                }

                m_timeManager.RefreshTasks();
                SaveTasks();
            }
        }

        #endregion

        private void SaveTasks()
        {
            Task.Factory.StartNew(() =>
            {
                m_timeManager.SaveTasks();
            })
            .ContinueWith(t =>
            {
                //TODO: Show save data icon
            }, TaskScheduler.FromCurrentSynchronizationContext());

        }
    }
}
