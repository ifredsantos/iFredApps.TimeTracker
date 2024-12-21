﻿using Microsoft.EntityFrameworkCore;
using iFredApps.TimeTracker.Core.Interfaces.Repository;
using iFredApps.TimeTracker.Core.Models;
using iFredApps.TimeTracker.Data.Data;

namespace iFredApps.TimeTracker.Data.Repositories
{
   public class WorkspaceRepository : IWorkspaceRepository
   {
      private readonly AppDbContext _context;

      public WorkspaceRepository(AppDbContext context)
      {
         _context = context;
      }

      public async Task<IEnumerable<Workspace>> GetAllByUserId(int user_id)
      {
         return await _context.Workspaces.Where(x => x.user_id == user_id).ToListAsync();
      }

      public async Task<Workspace> Create(Workspace workspace)
      {
         workspace.workspace_id = null;

         await _context.Workspaces.AddAsync(workspace);
         await _context.SaveChangesAsync();

         return workspace;
      }

      public async Task<Workspace> Update(Workspace workspace)
      {
         _context.Workspaces.Update(workspace);
         await _context.SaveChangesAsync();

         return workspace;
      }

      public async Task Delete(int workspace_id)
      {
         var workspace = await _context.Workspaces.FindAsync(workspace_id);
         if (workspace != null)
         {
            _context.Workspaces.Remove(workspace);
            await _context.SaveChangesAsync();
         }
      }
   }
}