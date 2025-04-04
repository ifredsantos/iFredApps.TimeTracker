using iFredApps.TimeTracker.Core.Interfaces.Repository;
using iFredApps.TimeTracker.Core.Models;
using iFredApps.TimeTracker.Data.Data;
using Microsoft.EntityFrameworkCore;

namespace iFredApps.TimeTracker.Data.Repositories
{
   public class SessionRepository : ISessionRepository
   {
      private readonly AppDbContext _context;

      public SessionRepository(AppDbContext context)
      {
         _context = context;
      }

      public async Task<IEnumerable<Session>> GetUserSessions(int user_id, int workspace_id, DateTime? start_date = null, DateTime? end_date = null)
      {
         if (start_date.HasValue && !end_date.HasValue)
            return await _context.Sessions.Where(x => x.user_id == user_id && x.workspace_id == workspace_id && x.start_date >= start_date).OrderByDescending(x => x.end_date).ToListAsync();
         else if (start_date.HasValue && end_date.HasValue)
            return await _context.Sessions.Where(x => x.user_id == user_id && x.workspace_id == workspace_id && x.start_date >= start_date && x.end_date <= end_date).OrderByDescending(x => x.end_date).ToListAsync();
         else
            return await _context.Sessions.Where(x => x.user_id == user_id && x.workspace_id == workspace_id).OrderByDescending(x => x.end_date).ToListAsync();
      }

      public async Task<Session> Create(Session session)
      {
         session.session_id = null;

         await _context.Sessions.AddAsync(session);
         await _context.SaveChangesAsync();

         return session;
      }

      public async Task<Session> Update(Session session)
      {
         _context.Sessions.Update(session);
         await _context.SaveChangesAsync();

         return session;
      }

      public async Task Delete(int session_id)
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
