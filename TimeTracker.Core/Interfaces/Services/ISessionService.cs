using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTracker.Core.Models;

namespace TimeTracker.Core.Interfaces.Services
{
   public interface ISessionService
   {
      Task<IEnumerable<Session>> GetUserSessions(int user_id);
      Task<Session> CreateSession(Session session);
      Task<Session> UpdateSession(Session session);
      Task DeleteSession(int session_id);
   }
}
