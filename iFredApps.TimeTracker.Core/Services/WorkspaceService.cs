using iFredApps.TimeTracker.Core.Interfaces.Repository;
using iFredApps.TimeTracker.Core.Interfaces.Services;
using iFredApps.TimeTracker.Core.Models;

namespace iFredApps.TimeTracker.Core.Services
{
   public class WorkspaceService : IWorkspaceService
   {
      private readonly IWorkspaceService _workspaceRepository;

      public WorkspaceService(IWorkspaceService sessionRepository)
      {
         _workspaceRepository = sessionRepository;
      }

      public Task<IEnumerable<Workspace>> GetAllByUserId(int user_id)
      {
         return _workspaceRepository.GetAllByUserId(user_id);
      }

      public async Task<Workspace> Create(Workspace workspace)
      {
         return await _workspaceRepository.Create(workspace);
      }

      public async Task<Workspace> Update(Workspace workspace)
      {
         return await _workspaceRepository.Update(workspace);
      }

      public async Task Delete(int workspace_id)
      {
         await _workspaceRepository.Delete(workspace_id);
      }
   }
}
