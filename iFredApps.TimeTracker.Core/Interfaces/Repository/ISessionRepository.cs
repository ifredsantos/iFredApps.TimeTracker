﻿using iFredApps.TimeTracker.Core.Models;

namespace iFredApps.TimeTracker.Core.Interfaces.Repository
{
   public interface ISessionRepository
   {
      Task<IEnumerable<Session>> GetUserSessions(int user_id);
      Task<Session> CreateSession(Session session);
      Task<Session> UpdateSession(Session session);
      Task DeleteSession(int session_id);
   }
}
