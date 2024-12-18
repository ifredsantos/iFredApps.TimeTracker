using iFredApps.TimeTracker.Core.Models;

namespace iFredApps.TimeTracker.Core.Interfaces.Services
{
   public interface IWorkspaceService
   {
      Task<IEnumerable<Workspace>> GetAllByUserId(int user_id);
      Task<Workspace> Create(Workspace workspace);
      Task<Workspace> Update(Workspace workspace);
      Task Delete(int workspace_id);
   }
}
