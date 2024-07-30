using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTracker.SL;
using TimeTracker.WebApi.Context;

namespace TimerTracker.WebApi.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   public class SessionsController : ControllerBase
   {
      private readonly SessionContext _context;

      public SessionsController(SessionContext context)
      {
         _context = context;
      }

      // GET: api/Sessions
      [HttpGet]
      public async Task<ActionResult<IEnumerable<sSession>>> GetSessions()
      {
         return await _context.sessions.ToListAsync();
      }

      // GET: api/Sessions/5
      [HttpGet("{id}")]
      public async Task<ActionResult<sSession>> GetSession(int id)
      {
         var session = await _context.sessions.FindAsync(id);

         if(session == null)
         {
            return NotFound();
         }

         return session;
      }

      // POST: api/Sessions
      [HttpPost]
      public async Task<ActionResult<sSession>> SaveSession(sSession session)
      {
         _context.sessions.Add(session);
         await _context.SaveChangesAsync();

         return CreatedAtAction("GetSession", new { id = session.session_id }, session);
      }

      // PUT: api/Sessions/5
      [HttpPut("{id}")]
      public async Task<IActionResult> UpdateSession(int id, sSession session)
      {
         if (id != session.session_id)
         {
            return BadRequest();
         }

         _context.Entry(session).State = EntityState.Modified;

         try
         {
            await _context.SaveChangesAsync();
         }
         catch (DbUpdateConcurrencyException)
         {
            if (!SessionExists(id))
            {
               return NotFound();
            }
            else
            {
               throw;
            }
         }

         return NoContent();
      }

      // DELETE: api/Sessions/5
      [HttpDelete("{id}")]
      public async Task<IActionResult> DeleteSession(int id)
      {
         var session = await _context.sessions.FindAsync(id);
         if (session == null)
         {
            return NotFound();
         }

         _context.sessions.Remove(session);
         await _context.SaveChangesAsync();

         return NoContent();
      }

      private bool SessionExists(int id)
      {
         return _context.sessions.Any(e => e.session_id == id);
      }
   }
}
