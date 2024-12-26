using iFredApps.TimeTracker.Core.Interfaces.Repository;
using iFredApps.TimeTracker.Core.Interfaces.Services;
using iFredApps.TimeTracker.Core.Models;

namespace iFredApps.TimeTracker.Core.Services
{
   public class UserService : IUserService
   {
      private readonly IUserRepository _userRepository;
      private readonly IWorkspaceRepository _workspaceRepository;

      public UserService(IUserRepository userRepository, IWorkspaceRepository workspaceRepository)
      {
         _userRepository = userRepository;
         _workspaceRepository = workspaceRepository;
      }

      public async Task<IEnumerable<User>> GetAllUsers()
      {
         return await _userRepository.GetAllUsers();
      }

      public async Task CreateUser(User user)
      {
         //Validate
         {
            User existingUserByUsername = await _userRepository.SearchUserByTerm(user.username);
            if (existingUserByUsername != null)
               throw new Exception("Unable to create user account as one already exists with the same username");

            User existingUserByEmail = await _userRepository.SearchUserByTerm(user.email);
            if (existingUserByEmail != null)
               throw new Exception("Unable to create user account as one already exists with the same email");
         }

         user.password = BCrypt.Net.BCrypt.HashPassword(user.password);
         var userSaved = await _userRepository.CreateUser(user);
         if (userSaved != null)
         {
            await _workspaceRepository.Create(new Workspace
            {
               user_id = userSaved.user_id,
               name = "My Workspace",
               is_default = true,
            });
         }
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
