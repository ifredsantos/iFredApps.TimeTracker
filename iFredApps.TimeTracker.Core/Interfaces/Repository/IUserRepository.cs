using iFredApps.TimeTracker.Core.Models;

namespace iFredApps.TimeTracker.Core.Interfaces.Repository
{
   public interface IUserRepository
   {
      Task<IEnumerable<User>> GetAllUsers();
      Task<User> SearchUserByTerm(string term);
      Task CreateUser(User user);
      Task UpdateUser(User user);
      Task DeleteUser(int user_id);
   }
}
