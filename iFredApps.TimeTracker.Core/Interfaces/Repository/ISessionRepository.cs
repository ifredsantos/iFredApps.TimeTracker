using iFredApps.TimeTracker.Core.Models;

namespace iFredApps.TimeTracker.Core.Interfaces.Repository
{
   public interface ISessionRepository
   {
      Task<IEnumerable<Session>> GetUserSessions(int user_id, int workspace_id, DateTime? start_date = null, DateTime? end_date = null);
      Task<IEnumerable<Session>> GetUserSessionsByDescription(int user_id, int workspace_id, string description, DateTime? start_date = null, DateTime? end_date = null);
      Task<Session> Create(Session session);
      Task<Session> Update(Session session);
      Task Delete(int session_id);
   }
}
