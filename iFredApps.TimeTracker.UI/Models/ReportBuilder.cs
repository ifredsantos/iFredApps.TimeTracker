using iFredApps.Lib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace iFredApps.TimeTracker.UI.Models
{
   public static class ReportBuilder
   {
      public static async Task<string> GenerateDailyReport(TimeManagerGroup group)
      {
         string fileContent = await File.ReadAllTextAsync(@"HTMLTemplates\DailyReport.html", Encoding.UTF8);
         if(!string.IsNullOrEmpty(fileContent))
         {
            fileContent = fileContent.Replace("{DATE}", group.date_group_reference.ToString("dd/MM/yyyy"));
            fileContent = fileContent.Replace("{DAY_TOTAL_TIME}", group.total_time.ToString(@"hh\:mm\:ss"));
            fileContent = fileContent.Replace("{USER_NAME}", AppWebClient.Instance.GetLoggedUserData().name);

            string htmlBodyContent = "";
            foreach (TimeManagerTask task in group.tasks)
            {
               htmlBodyContent += GenerateSessionGroup(task);
            }

            fileContent = fileContent.Replace("{BODY_CONTENT}", htmlBodyContent);
         }
         return fileContent;
      }
      
      private static string GenerateSessionGroup(TimeManagerTask task)
      {
         string html = "";

         html = $@"
            <tr>
                <td>{task.description}</td>
                <td class=""text-right"">{task.session_total_time.ToString(@"hh\:mm\:ss")}</td>
            </tr>";

         if(!task.sessions.IsNullOrEmpty())
         {
            html += $@"
               <tr>
                   <td colspan=""2"">
                       <table class=""task-table task-table-detail"">
                           <tr class=""header"">
                               <th>Start Date</th>
                               <th>End Date</th>
                               <th>Total Time</th>
                           </tr>";

            foreach (var session in task.sessions)
            {
               html += $@"
                           <tr>
                               <td>{session.start_date.ToString("dd/MM/yyyy HH:mm:ss")}</td>
                               <td>{(session.end_date.HasValue ? session.end_date.Value.ToString("dd/MM/yyyy HH:mm:ss") : "in progress")}</td>
                               <td>{session.total_time.ToString(@"hh\:mm\:ss")}</td>
                           </tr>";

               if (!string.IsNullOrEmpty(session.observation))
               {
                  html += $@"
                           <tr>
                               <td colspan=""3"">
                                   <small>Description: </small>
                                   <small>{session.observation}</small>
                               </td>
                           </tr>";
               }
            }

            html += $@"
                       </table>
                   </td>
               </tr>";
         }
            

         return html;
      }
   }
}
