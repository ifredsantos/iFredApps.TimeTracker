using iFredApps.TimeTracker.Core.Models;

namespace iFredApps.TimeTracker.Core.Interfaces.Repository
{
   public interface IUserRepository
   {
      Task<IEnumerable<User>> GetAllUsers();
      Task<User> GetUser(int user_id);
      Task<User> SearchUserByTerm(string term);
      Task<User> CreateUser(User user);
      Task<User> UpdateUser(User user);
      Task DeleteUser(int user_id);
   }
}
