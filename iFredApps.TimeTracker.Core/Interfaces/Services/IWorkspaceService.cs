using iFredApps.TimeTracker.Core.Models;

namespace iFredApps.TimeTracker.Core.Interfaces.Services
{
   public interface IWorkspaceService
   {
      Task<Result<IEnumerable<Workspace>>> GetAllByUserId(int user_id);
      Task<Result<Workspace>> Create(Workspace workspace);
      Task<Result<Workspace>> Update(Workspace workspace);
      Task<Result<bool>> Delete(int workspace_id);
   }
}
