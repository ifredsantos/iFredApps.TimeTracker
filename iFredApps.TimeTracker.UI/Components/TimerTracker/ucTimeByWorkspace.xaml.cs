using iFredApps.Lib;
using iFredApps.Lib.Wpf.Execption;
using iFredApps.Lib.Wpf.Messages;
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
using System.Windows.Threading;
using static iFredApps.TimeTracker.UI.Models.WebApiCall;

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

         DataContextChanged += UcTimeByWorkspace_DataContextChanged;
      }

      private async void UcTimeByWorkspace_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
      {
         try
         {
            _tmByWorkspace = DataContext as TimeManager;

            if (_tmByWorkspace == null)
               return;

            _tmByWorkspace.NotifyValue(nameof(_tmByWorkspace.isLoading), true);

            if (_tmByWorkspace.uncompleted_session != null)
            {
               await Dispatcher.InvokeAsync(async () =>
               {
                  if (MessageBox.Show(
                     string.Format("There is one session to complete \"{0}\". Do you want to resume?", _tmByWorkspace.uncompleted_session.description),
                     "Calm down!",
                     MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes
                  ) == MessageBoxResult.Yes)
                  {
                     _tmByWorkspace.current_session = new TimeManagerTaskSession
                     {
                        session_id = _tmByWorkspace.uncompleted_session.session_id,
                        user_id = _tmByWorkspace.uncompleted_session.user_id,
                        description = _tmByWorkspace.uncompleted_session.description,
                        start_date = _tmByWorkspace.uncompleted_session.start_date,
                        observation = _tmByWorkspace.uncompleted_session.observation,
                        total_time = Utilities.GetDateTimeNow() - _tmByWorkspace.uncompleted_session.start_date
                     };
                     _tmByWorkspace.NotifyValue(nameof(_tmByWorkspace.current_session));
                     _tmByWorkspace.NotifyValue(nameof(_tmByWorkspace.uncompleted_session), null);
                     timeRowEditor.StartStopSession();
                  }
                  else
                  {
                     _tmByWorkspace.current_session = new TimeManagerTaskSession();

                     await DeleteSession(_tmByWorkspace.uncompleted_session.session_id.Value);
                  }
               }, DispatcherPriority.Background);
            }

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

      #region Private Methods

      private async Task GroupingSessionIntoTasks()
      {
         _tmByWorkspace.task_groups.Clear();

         if (_tmByWorkspace.sessions != null)
         {
            Dictionary<DateTime, List<TimeManagerTask>> dicTasksByDate = new Dictionary<DateTime, List<TimeManagerTask>>();

            //Default Group
            dicTasksByDate.Add(Utilities.GetDateTimeNow().Date, new List<TimeManagerTask>());

            foreach (var session in _tmByWorkspace.sessions.OrderByDescending(x => x.end_date))
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

      private async void OnTaskGetStatisticsClick(object sender, TimeTaskRemoveEventArgs e)
      {
         try
         {
            if (e.TaskData != null)
            {
               var referenceSession = e.TaskData.sessions.FirstOrDefault();
               var sessions = await WebApiCall.Sessions.GetSessionsByDescription(AppWebClient.Instance.GetClient(), new GetSessionsRequest
               {
                  user_id = referenceSession.user_id,
                  workspace_id = referenceSession.workspace_id.Value,
                  description = e.TaskData.description,
               });

               // Calculate total time for all returned sessions
               TimeSpan totalTime = TimeSpan.Zero;
               var list = sessions?.TrataResposta();
               if (list != null)
               {
                  foreach (var s in list)
                  {
                     if (s.start_date != null)
                     {
                        DateTime start = s.start_date;
                        DateTime end = s.end_date ?? Utilities.GetDateTimeNow();
                        if (end > start)
                        {
                           s.total_time = (end - start);
                           totalTime += (end - start);
                        }
                     }
                  }
               }

               // totalTime now contains the summed duration of all sessions
               double totalTimeInHours = Math.Round(totalTime.TotalHours, 2);
               Message.Success($"You have spend {totalTimeInHours} hours on project '{e.TaskData.description}'.", "Summary of statistics");
               // totalTimeInHours now contains the duration in hours (rounded to 2 decimals)
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
