using iFredApps.TimeTracker.Core.Interfaces.Repository;
using iFredApps.TimeTracker.Core.Models;
using iFredApps.TimeTracker.Data.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace iFredApps.TimeTracker.Data.Repositories
{
   public class SessionRepository : ISessionRepository
   {
      private readonly AppDbContext _context;

      public SessionRepository(AppDbContext context)
      {
         _context = context;
      }

      private IQueryable<Session> BuildUserSessionsQuery(int userId, int workspaceId, string? description, DateTime? startDate, DateTime? endDate)
      {
         var query = _context.Sessions
            .AsNoTracking()
            .Where(x => x.user_id == userId && x.workspace_id == workspaceId);

         if (!string.IsNullOrWhiteSpace(description))
         {
            var pattern = $"%{description.Trim()}%";
            query = query.Where(x => x.description != null && EF.Functions.Like(x.description, pattern));
         }

         if (startDate.HasValue && !endDate.HasValue)
            query = query.Where(x => x.start_date >= startDate.Value);
         else if (startDate.HasValue && endDate.HasValue)
            query = query.Where(x => x.start_date >= startDate.Value && x.end_date <= endDate.Value);

         return query;
      }

      public async Task<IEnumerable<Session>> GetUserSessions(int user_id, int workspace_id, DateTime? start_date = null, DateTime? end_date = null)
      {
         var query = BuildUserSessionsQuery(user_id, workspace_id, null, start_date, end_date)
            .OrderByDescending(x => x.end_date);

         return await query.ToListAsync();
      }

      public async Task<IEnumerable<Session>> GetUserSessionsByDescription(int user_id, int workspace_id, string description, DateTime? start_date = null, DateTime? end_date = null)
      {
         var query = BuildUserSessionsQuery(user_id, workspace_id, description, start_date, end_date)
            .OrderByDescending(x => x.end_date);

         return await query.ToListAsync();
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
