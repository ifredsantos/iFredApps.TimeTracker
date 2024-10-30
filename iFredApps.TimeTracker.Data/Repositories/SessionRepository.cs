using Microsoft.EntityFrameworkCore;
using iFredApps.TimeTracker.Core.Interfaces.Repository;
using iFredApps.TimeTracker.Core.Models;
using iFredApps.TimeTracker.Data.Data;

namespace iFredApps.TimeTracker.Data.Repositories
{
   public class SessionRepository : ISessionRepository
   {
      private readonly AppDbContext _context;

      public SessionRepository(AppDbContext context)
      {
         _context = context;
      }

      public async Task<IEnumerable<Session>> GetUserSessions(int user_id)
      {
         return await _context.Sessions.Where(x => x.user_id == user_id).ToListAsync();
      }

      public async Task<Session> CreateSession(Session session)
      {
         await _context.Sessions.AddAsync(session);
         await _context.SaveChangesAsync();

         return session;
      }

      public async Task<Session> UpdateSession(Session session)
      {
         _context.Sessions.Update(session);
         await _context.SaveChangesAsync();

         return session;
      }

      public async Task DeleteSession(int session_id)
      {
         var session = await _context.Sessions.FindAsync(session_id);
         if (session != null)
         {
            _context.Sessions.Remove(session);
            await _context.SaveChangesAsync();
         }
      }
   }
}
