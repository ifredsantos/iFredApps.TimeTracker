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

      public async Task<Result<IEnumerable<User>>> GetAllUsers()
      {
         return Result<IEnumerable<User>>.Ok(await _userRepository.GetAllUsers());
      }

      public async Task<Result<User>> GetUser(int user_id)
      {
         return Result<User>.Ok(await _userRepository.GetUser(user_id));
      }

      public async Task<Result<User>> CreateUser(User user)
      {
         //Validate
         {
            User existingUserByUsername = await _userRepository.SearchUserByTerm(user.username);
            if (existingUserByUsername != null)
               return Result<User>.Fail($"A user with the username '{user.username}' already exists.");

            User existingUserByEmail = await _userRepository.SearchUserByTerm(user.email);
            if (existingUserByEmail != null)
               return Result<User>.Fail($"A user with the email '{user.email}' already exists.");
         }

         user.created_at = DateTime.UtcNow;

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

         return Result<User>.Ok(userSaved);
      }

      public async Task<Result<User>> UpdateUser(User user)
      {
         if (!string.IsNullOrEmpty(user.password))
         {
            user.password = BCrypt.Net.BCrypt.HashPassword(user.password);
         }
         var userSaved = await _userRepository.UpdateUser(user);
         return Result<User>.Ok(userSaved);
      }

      public async Task<Result<bool>> DeleteUser(int user_id)
      {
         await _userRepository.DeleteUser(user_id);
         return Result<bool>.Ok(true);
      }

      public async Task<Result<User>> ValidateUser(string userSearchKey, string plainPassword)
      {
         User user = await _userRepository.SearchUserByTerm(userSearchKey);

         if (user != null && BCrypt.Net.BCrypt.Verify(plainPassword, user.password))
         {
            return Result<User>.Ok(user);
         }
         else
         {
            return Result<User>.Fail("Invalid credentials");
         }
      }
   }
}
