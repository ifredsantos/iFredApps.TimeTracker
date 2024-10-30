using iFredApps.TimeTracker.Core.Interfaces.Repository;
using iFredApps.TimeTracker.Core.Interfaces.Services;
using iFredApps.TimeTracker.Core.Models;

namespace iFredApps.TimeTracker.Core.Services
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

      public async Task<Session> CreateSession(Session session)
      {
         return await _sessionRepository.CreateSession(session);
      }

      public async Task<Session> UpdateSession(Session session)
      {
         return await _sessionRepository.UpdateSession(session);
      }

      public async Task DeleteSession(int session_id)
      {
         await _sessionRepository.DeleteSession(session_id);
      }
   }
}
