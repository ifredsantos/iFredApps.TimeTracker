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

      public async Task<Session> Create(Session session)
      {
         return await _sessionRepository.Create(session);
      }

      public async Task<Session> Update(Session session)
      {
         return await _sessionRepository.Update(session);
      }

      public async Task Delete(int session_id)
      {
         await _sessionRepository.Delete(session_id);
      }
   }
}
