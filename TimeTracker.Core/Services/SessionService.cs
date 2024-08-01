using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTracker.Core.Interfaces.Repository;
using TimeTracker.Core.Interfaces.Services;
using TimeTracker.Core.Models;

namespace TimeTracker.Core.Services
{
   public class SessionService : ISessionService
   {
      private readonly ISessionRepository _sessionRepository;

      public SessionService(ISessionRepository sessionRepository)
      {
         _sessionRepository = sessionRepository;
      }

      public Task<IEnumerable<Session>> GetUserSessions(int user_id)
      {
         return _sessionRepository.GetUserSessions(user_id);
      }

      public async Task CreateSession(Session session)
      {
         await _sessionRepository.CreateSession(session);
      }

      public async Task UpdateSession(Session session)
      {
         await _sessionRepository.UpdateSession(session);
      }

      public async Task DeleteSession(int session_id)
      {
         await _sessionRepository.DeleteSession(session_id);
      }
   }
}
