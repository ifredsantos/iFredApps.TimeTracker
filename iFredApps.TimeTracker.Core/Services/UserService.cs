using iFredApps.TimeTracker.Core.Interfaces.Repository;
using iFredApps.TimeTracker.Core.Interfaces.Services;
using iFredApps.TimeTracker.Core.Models;

namespace iFredApps.TimeTracker.Core.Services
{
   public class UserService : IUserService
   {
      private readonly IUserRepository _userRepository;

      public UserService(IUserRepository userRepository)
      {
         _userRepository = userRepository;
      }

      public async Task<IEnumerable<User>> GetAllUsers()
      {
         return await _userRepository.GetAllUsers();
      }

      public async Task CreateUser(User user)
      {
         user.password = BCrypt.Net.BCrypt.HashPassword(user.password);
         await _userRepository.CreateUser(user);
      }

      public async Task UpdateUser(User user)
      {
         if (!string.IsNullOrEmpty(user.password))
         {
            user.password = BCrypt.Net.BCrypt.HashPassword(user.password);
         }
         await _userRepository.UpdateUser(user);
      }

      public async Task DeleteUser(int user_id)
      {
         await _userRepository.DeleteUser(user_id);
      }

      public async Task<User> ValidateUser(string userSearchKey, string plainPassword)
      {
         User user = await _userRepository.SearchUserByTerm(userSearchKey);

         if (user != null && BCrypt.Net.BCrypt.Verify(plainPassword, user.password))
         {
            return user;
         }
         else
         {
            return null;
         }
      }
   }
}
