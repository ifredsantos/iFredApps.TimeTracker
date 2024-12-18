using Microsoft.AspNetCore.Mvc;
using iFredApps.TimeTracker.Core.Interfaces.Services;
using iFredApps.TimeTracker.Core.Models;
using Microsoft.AspNetCore.Authorization;

namespace iFredApps.TimeTracker.WebApi.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   public class WorkspaceController : ControllerBase
   {
      private readonly IWorkspaceService _workspaceService;

      public WorkspaceController(IWorkspaceService sessionService)
      {
         _workspaceService = sessionService;
      }

      [HttpGet("GetAllByUserId/{user_id}")]
      //[Authorize]
      public async Task<ActionResult<IEnumerable<Session>>> GetAllByUserId(int user_id)
      {
         var workspaces = await _workspaceService.GetAllByUserId(user_id);
         return Ok(workspaces);
      }

      [HttpPost("Create")]
      //[Authorize]
      public async Task<ActionResult<Session>> Create([FromBody] Workspace workspace)
      {
         if (workspace == null || !ModelState.IsValid)
         {
            return BadRequest("Invalid workspace data.");
         }

         var result = await _workspaceService.Create(workspace);

         return Ok(result);
      }

      [HttpPut("Update/{id}")]
      //[Authorize]
      public async Task<ActionResult<Session>> Update(int id, [FromBody] Workspace workspace)
      {
         if (id != workspace.workspace_id)
         {
            return BadRequest();
         }
         var result = await _workspaceService.Update(workspace);
         return Ok(result);
      }

      [HttpDelete("Delete/{id}")]
      //[Authorize]
      public async Task<ActionResult> Delete(int id)
      {
         await _workspaceService.Delete(id);
         return NoContent();
      }
   }
}
