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

      public async Task<Result<IEnumerable<Session>>> GetUserSessions(int user_id, int workspace_id)
      {
         return Result<IEnumerable<Session>>.Ok(await _sessionRepository.GetUserSessions(user_id, workspace_id));
      }

      public async Task<Result<Session>> Create(Session session)
      {
         return Result<Session>.Ok(await _sessionRepository.Create(session));
      }

      public async Task<Result<Session>> Update(Session session)
      {
         return Result<Session>.Ok(await _sessionRepository.Update(session));
      }

      public async Task<Result<bool>> Delete(int session_id)
      {
         await _sessionRepository.Delete(session_id);
         return Result<bool>.Ok(true);
      }
   }
}
