using iFredApps.TimeTracker.Core.Models;

namespace iFredApps.TimeTracker.Core.Interfaces.Services
{
   public interface IUserService
   {
      Task<Result<IEnumerable<User>>> GetAllUsers();
      Task<Result<User>> GetUser(int user_id);
      Task<Result<User>> CreateUser(User user);
      Task<Result<User>> UpdateUser(User user);
      Task<Result<bool>> DeleteUser(int user_id);
      Task<Result<User>> ValidateUser(string userSearchKey, string plainPassword);

      // Password reset
      Task<Result<bool>> InitiatePasswordReset(string email);
      Task<Result<bool>> CompletePasswordReset(string token, string newPassword);
   }
}
