using iFredApps.TimeTracker.Core.Interfaces.Repository;
using iFredApps.TimeTracker.Core.Models;
using iFredApps.TimeTracker.Data.Data;
using Microsoft.EntityFrameworkCore;

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

      public async Task<User> GetUser(int user_id)
      {
         return await _context.Users.FirstOrDefaultAsync(x => x.user_id == user_id);
      }

      public async Task<User> SearchUserByTerm(string term)
      {
         return await _context.Users.FirstOrDefaultAsync(x => x.username == term || x.email == term);
      }

      public async Task<User> FindByPasswordResetToken(string token)
      {
         return await _context.Users.FirstOrDefaultAsync(x => x.password_reset_token == token && x.password_reset_expires_at != null && x.password_reset_expires_at > DateTime.UtcNow);
      }

      public async Task<User> CreateUser(User user)
      {
         var userSaved = await _context.Users.AddAsync(user);
         await _context.SaveChangesAsync();

         return userSaved.Entity;
      }

      public async Task<User> UpdateUser(User user)
      {
         var userSaved = _context.Users.Update(user);
         await _context.SaveChangesAsync();

         return userSaved.Entity;
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
