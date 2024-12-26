using iFredApps.TimeTracker.Core.Models;

namespace iFredApps.TimeTracker.Core.Interfaces.Services
{
   public interface ISessionService
   {
      Task<Result<IEnumerable<Session>>> GetUserSessions(int user_id, int workspace_id);
      Task<Result<Session>> Create(Session session);
      Task<Result<Session>> Update(Session session);
      Task<Result<bool>> Delete(int session_id);
   }
}
