﻿using Microsoft.EntityFrameworkCore;
using iFredApps.TimeTracker.Core.Interfaces.Repository;
using iFredApps.TimeTracker.Core.Models;
using iFredApps.TimeTracker.Data.Data;

namespace iFredApps.TimeTracker.Data.Repositories
{
   public class UserRepository : IUserRepository
   {
      private readonly AppDbContext _context;

      public UserRepository(AppDbContext context)
      {
         _context = context;
      }

      public async Task<IEnumerable<User>> GetAllUsers()
      {
         return await _context.Users.ToListAsync();
      }

      public async Task<User> SearchUserByTerm(string term)
      {
         return await _context.Users.FirstOrDefaultAsync(x => x.username == term || x.email == term);
      }

      public async Task CreateUser(User user)
      {
         await _context.Users.AddAsync(user);
         await _context.SaveChangesAsync();
      }

      public async Task UpdateUser(User user)
      {
         _context.Users.Update(user);
         await _context.SaveChangesAsync();
      }

      public async Task DeleteUser(int id)
      {
         var user = await _context.Users.FindAsync(id);
         if (user != null)
         {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
         }
      }
   }
}