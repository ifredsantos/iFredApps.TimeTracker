using Microsoft.AspNetCore.Mvc;
using iFredApps.TimeTracker.Core.Interfaces.Services;
using iFredApps.TimeTracker.Core.Models;

namespace iFredApps.TimeTracker.WebApi.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   public class SessionController : ControllerBase
   {
      private readonly ISessionService _sessionService;

      public SessionController(ISessionService sessionService)
      {
         _sessionService = sessionService;
      }

      [HttpGet("GetSessions/{user_id}/{workspace_id}")]
      //[Authorize]
      public async Task<ActionResult<IEnumerable<Session>>> GetSessions(int user_id, int workspace_id)
      {
         var sessions = await _sessionService.GetUserSessions(user_id, workspace_id);
         return Ok(sessions);
      }

      [HttpPost("CreateSession")]
      public async Task<ActionResult<Session>> CreateSession([FromBody] Session session)
      {
         if (session == null || !ModelState.IsValid)
         {
            return BadRequest("Invalid session data.");
         }

         var result = await _sessionService.Create(session);

         return Ok(result);
      }

      [HttpPut("UpdateSession/{id}")]
      //[Authorize]
      public async Task<ActionResult<Session>> UpdateSession(int id, [FromBody] Session session)
      {
         if (id != session.session_id)
         {
            return BadRequest();
         }
         var result = await _sessionService.Update(session);
         return Ok(result);
      }

      [HttpDelete("DeleteSession/{id}")]
      //[Authorize]
      public async Task<ActionResult> DeleteSession(int id)
      {
         await _sessionService.Delete(id);
         return NoContent();
      }
   }
}
