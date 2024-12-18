using iFredApps.TimeTracker.Core.Models;

namespace iFredApps.TimeTracker.Core.Interfaces.Repository
{
   public interface ISessionRepository
   {
      Task<IEnumerable<Session>> GetUserSessions(int user_id);
      Task<Session> Create(Session session);
      Task<Session> Update(Session session);
      Task Delete(int session_id);
   }
}
