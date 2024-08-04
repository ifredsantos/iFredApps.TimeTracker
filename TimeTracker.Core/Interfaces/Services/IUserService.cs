using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTracker.Core.Models;

namespace TimeTracker.Core.Interfaces.Services
{
   public interface IUserService
   {
      Task<IEnumerable<User>> GetAllUsers();
      Task CreateUser(User user);
      Task UpdateUser(User user);
      Task DeleteUser(int user_id);
      Task<User> ValidateUser(string userSearchKey, string plainPassword);
   }
}
