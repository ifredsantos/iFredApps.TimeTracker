using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TimeTracker.Core.Interfaces.Services;
using TimeTracker.Core.Models;
using TimeTracker.Core.Services;

namespace TimerTracker.WebApi.Controllers
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

      [HttpGet("GetSessions")]
      //[Authorize]
      public async Task<ActionResult<IEnumerable<Session>>> GetSessions(int user_id)
      {
         var users = await _sessionService.GetUserSessions(user_id);
         return Ok(users);
      }

      [HttpPost]
      public async Task<ActionResult> CreateSession([FromBody] Session session)
      {
         if (session == null || !ModelState.IsValid)
         {
            return BadRequest("Invalid user data.");
         }

         await _sessionService.CreateSession(session);

         return Ok("Session created successfully.");
      }

      [HttpPut("{id}")]
      //[Authorize]
      public async Task<ActionResult> UpdateSession(int id, [FromBody] Session session)
      {
         if (id != session.session_id)
         {
            return BadRequest();
         }
         await _sessionService.UpdateSession(session);
         return NoContent();
      }

      [HttpDelete("{id}")]
      //[Authorize]
      public async Task<ActionResult> DeleteSession(int id)
      {
         await _sessionService.DeleteSession(id);
         return NoContent();
      }
   }
}
