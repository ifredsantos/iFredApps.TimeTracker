﻿using iFredApps.Lib;
using iFredApps.Lib.Wpf.Execption;
using iFredApps.TimeTracker.UI.Models;
using iFredApps.TimeTracker.UI.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace iFredApps.TimeTracker.UI.Components.TimerTracker
{
   /// <summary>
   /// Interaction logic for ucTimeByWorkspace.xaml
   /// </summary>
   public partial class ucTimeByWorkspace : UserControl
   {
      public event EventHandler<NotificationEventArgs> OnNotificationShow;

      private TimeManager _tmByWorkspace = null;
      private bool _isFirstLoadComplete = false;

      public ucTimeByWorkspace()
      {
         InitializeComponent();

         Loaded += UcTimeManagerView_Loaded;
      }

      #region Private Methods

      private async Task InitData()
      {
         try
         {
            _tmByWorkspace = DataContext as TimeManager;

            if (_tmByWorkspace == null)
               return;

            _tmByWorkspace.NotifyValue(nameof(_tmByWorkspace.isLoading), true);

            DateTime startDate = Utilities.GetDateTimeNow().AddDays(-7);
            DateTime? endDate = null;
            var sessionsResult = await WebApiCall.Sessions.GetSessions(AppWebClient.Instance.GetClient(), AppWebClient.Instance.GetLoggedUserData().user_id, _tmByWorkspace.workspace.workspace_id.Value, startDate, endDate);

            var sessions = sessionsResult.TrataResposta();

            var data = new TimeManagerDatabaseData
            {
               sessions = sessions,
               uncompleted_session = sessions?.Find(x => x.end_date == null)
            };

            if (data != null)
            {
               if (data.sessions != null && data.sessions.Count > 0)
               {
                  foreach (var session in data.sessions)
                  {
                     if (session.end_date.HasValue)
                        session.total_time = session.end_date.Value - session.start_date;

                     _tmByWorkspace.sessions.Add(session);
                  }
               }

               if (data.uncompleted_session != null)
               {
                  if (MessageBox.Show(string.Format("There is one session to complete \"{0}\". Do you want to resume?", data.uncompleted_session.description), "Calm down!",
                          MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes
                      ) == MessageBoxResult.Yes)
                  {
                     _tmByWorkspace.current_session = new TimeManagerTaskSession
                     {
                        session_id = data.uncompleted_session.session_id,
                        user_id = data.uncompleted_session.user_id,
                        description = data.uncompleted_session.description,
                        start_date = data.uncompleted_session.start_date,
                        observation = data.uncompleted_session.observation,
                        total_time = Utilities.GetDateTimeNow() - data.uncompleted_session.start_date
                     };
                     _tmByWorkspace.NotifyValue(nameof(_tmByWorkspace.current_session));

                     timeRowEditor.StartStopSession();
                  }
                  else
                  {
                     _tmByWorkspace.current_session = new TimeManagerTaskSession();

                     await DeleteSession(data.uncompleted_session.session_id.Value);
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
            _tmByWorkspace?.NotifyValue(nameof(_tmByWorkspace.isLoading), false);
         }
      }

      private async Task GroupingSessionIntoTasks()
      {
         _tmByWorkspace.task_groups.Clear();

         if (_tmByWorkspace.sessions != null)
         {
            Dictionary<DateTime, List<TimeManagerTask>> dicTasksByDate = new Dictionary<DateTime, List<TimeManagerTask>>();

            //Default Group
            dicTasksByDate.Add(Utilities.GetDateTimeNow().Date, new List<TimeManagerTask>());

            foreach (var session in _tmByWorkspace.sessions)
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
            _tmByWorkspace.task_groups.AddRange(lstGroups);
         }
      }

      private async Task<TimeManagerTaskSession> CreateSession(TimeManagerTaskSession session)
      {
         TimeManagerTaskSession result = null;

         session.user_id = AppWebClient.Instance.GetLoggedUserData().user_id;

         var createResult = await WebApiCall.Sessions.CreateSession(AppWebClient.Instance.GetClient(), session);
         result = createResult?.TrataResposta();
         if (createResult != null && createResult.Success)
         {
            OnNotificationShow?.Invoke(null, new NotificationEventArgs("Data synchronized successfully!", 3));
         }

         return result;
      }

      public async Task<TimeManagerTaskSession> UpdateSession(TimeManagerTaskSession session)
      {
         TimeManagerTaskSession result = null;

         session.user_id = AppWebClient.Instance.GetLoggedUserData().user_id;

         var updateResult = await WebApiCall.Sessions.UpdateSession(AppWebClient.Instance.GetClient(), session);
         result = updateResult?.TrataResposta();

         if (updateResult != null && updateResult.Success)
         {
            OnNotificationShow?.Invoke(null, new NotificationEventArgs("Data synchronized successfully!", 3));
         }

         return result;
      }

      public async Task DeleteSession(int sessionID)
      {
         await WebApiCall.Sessions.DeleteSession(AppWebClient.Instance.GetClient(), sessionID);

         OnNotificationShow?.Invoke(null, new NotificationEventArgs("Data synchronized successfully!", 3));
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
            e.SessionData.workspace_id = _tmByWorkspace.workspace.workspace_id;

            await UpdateSession(e.SessionData);

            _tmByWorkspace.sessions.Add(e.SessionData);

            _tmByWorkspace.NotifyValue(nameof(_tmByWorkspace.current_session), new TimeManagerTaskSession());

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
               if (_tmByWorkspace.current_session != null && _tmByWorkspace.current_session.is_working)
               {
                  MessageBox.Show("There is already a session in progress. Please stop the current session and try again.", "Calm down!", MessageBoxButton.OK, MessageBoxImage.Warning);
                  return;
               }
               else
               {
                  _tmByWorkspace.NotifyValue(nameof(_tmByWorkspace.current_session), new TimeManagerTaskSession
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
                     await DeleteSession(session.session_id.Value);
                     _tmByWorkspace.sessions.Remove(session);
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
                  session.workspace_id = _tmByWorkspace.workspace.workspace_id;

                  await UpdateSession(session);
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
               e.Session.workspace_id = _tmByWorkspace.workspace.workspace_id;
               await UpdateSession(e.Session);

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
               await DeleteSession(e.Session.session_id.Value);
               _tmByWorkspace.sessions.Remove(e.Session);

               await GroupingSessionIntoTasks();
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private async void OnSendReport(object sender, TimeTaskGroupArgs e)
      {
         try
         {
            if (e.Group != null)
            {
               string reportContent = await ReportBuilder.GenerateDailyReport(e.Group);
               //TODO: Send report to someone
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private async void OnDownloadReport(object sender, TimeTaskGroupArgs e)
      {
         try
         {
            if (e.Group != null)
            {
               string reportContent = await ReportBuilder.GenerateDailyReport(e.Group);

               SaveFileDialog saveFileDialog = new SaveFileDialog
               {
                  FileName = $"{e.Group.date_group_reference:dd-MM-yyyy} Daily Report - Time Tracker.html",
                  Filter = "HTML Files (*.html)|*.html|All Files (*.*)|*.*",
                  DefaultExt = "html"
               };

               if (saveFileDialog.ShowDialog() == true)
               {
                  try
                  {
                     await File.WriteAllTextAsync(saveFileDialog.FileName, reportContent);
                     MessageBox.Show("Report saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                  }
                  catch (Exception ex)
                  {
                     MessageBox.Show($"Error saving report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                  }
               }
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private async void SessionStarts(object sender, TimeRowSessionEventArgs e)
      {
         try
         {
            _tmByWorkspace = DataContext as TimeManager;

            if (_tmByWorkspace.current_session.session_id == null || _tmByWorkspace.current_session.session_id == 0) //only record if it is a new session (not a recovered session)
            {
               _tmByWorkspace.current_session.workspace_id = _tmByWorkspace.workspace.workspace_id;

               var sessionData = await CreateSession(_tmByWorkspace.current_session);
               if (sessionData != null)
               {
                  _tmByWorkspace.current_session.session_id = sessionData.session_id;
                  _tmByWorkspace.current_session.user_id = sessionData.user_id;
               }
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
