using Microsoft.AspNetCore.Mvc;
using TimeTracker.Core.Interfaces.Services;
using TimeTracker.Core.Models;

namespace TimeTracker.WebApi.Controllers
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

      [HttpGet("GetSessions/{user_id}")]
      //[Authorize]
      public async Task<ActionResult<IEnumerable<Session>>> GetSessions(int user_id)
      {
         var users = await _sessionService.GetUserSessions(user_id);
         return Ok(users);
      }

      [HttpPost("CreateSession")]
      public async Task<ActionResult<Session>> CreateSession([FromBody] Session session)
      {
         if (session == null || !ModelState.IsValid)
         {
            return BadRequest("Invalid user data.");
         }

         var result = await _sessionService.CreateSession(session);

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
         var result = await _sessionService.UpdateSession(session);
         return Ok(result);
      }

      [HttpDelete("DeleteSession/{id}")]
      //[Authorize]
      public async Task<ActionResult> DeleteSession(int id)
      {
         await _sessionService.DeleteSession(id);
         return NoContent();
      }
   }
}
