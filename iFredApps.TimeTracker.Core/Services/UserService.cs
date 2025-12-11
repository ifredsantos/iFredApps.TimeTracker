using iFredApps.TimeTracker.Core.Interfaces.Repository;
using iFredApps.TimeTracker.Core.Interfaces.Services;
using iFredApps.TimeTracker.Core.Models;
using System.IO;

namespace iFredApps.TimeTracker.Core.Services
{
   public class UserService : IUserService
   {
      private readonly IUserRepository _userRepository;
      private readonly IWorkspaceRepository _workspaceRepository;
      private readonly IEmailService _emailService;

      private readonly string _templatesPath;

      public UserService(IUserRepository userRepository, IWorkspaceRepository workspaceRepository, IEmailService emailService)
      {
         _userRepository = userRepository;
         _workspaceRepository = workspaceRepository;
         _emailService = emailService;

         // locate templates inside the Core project EmailTemplates folder
         _templatesPath = Path.Combine(AppContext.BaseDirectory, "EmailTemplates");
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

         user.password = BCrypt.Net.BCrypt.HashPassword(user.password.Trim());

         var userSaved = await _userRepository.CreateUser(user);
         if (userSaved != null)
         {
            await _workspaceRepository.Create(new Workspace
            {
               user_id = userSaved.user_id,
               name = "My Workspace",
               is_default = true,
               created_at = DateTime.UtcNow,
            });

            //Send email (fire-and-forget)
            {
               string templateFile = Path.Combine(_templatesPath, "PasswordRecoveryRequest.html");
               if (File.Exists(templateFile))
               {
                  string emailBody = File.ReadAllText(templateFile)
                     .Replace("{{USER}}", userSaved.username)
                     .Replace("{{CODE}}", "")
                     .Replace("{{EXPIRY_MINUTES}}", "");

                  _ = Task.Run(async () =>
                  {
                     try
                     {
                        await _emailService.SendEmailAsync(userSaved.email, "Confirmação de Conta", emailBody);
                     }
                     catch (Exception ex)
                     {
                        Console.WriteLine($"Failed to send signup email to {userSaved.email}: {ex.Message}");
                     }
                  });
               }
            }
         }

         return Result<User>.Ok(userSaved);
      }

      public async Task<Result<User>> UpdateUser(User user)
      {
         if (!string.IsNullOrEmpty(user.password))
         {
            user.password = BCrypt.Net.BCrypt.HashPassword(user.password.Trim());
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
         plainPassword = plainPassword.Trim();
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

      public async Task<Result<bool>> InitiatePasswordReset(string email)
      {
         if (string.IsNullOrEmpty(email))
            return Result<bool>.Fail("Email is required");

         var user = await _userRepository.SearchUserByTerm(email);
         if (user == null)
            return Result<bool>.Fail("No user found with that email");

         // Generate a simple numeric code (6 digits) and expiry (15 minutes)
         int code = System.Security.Cryptography.RandomNumberGenerator.GetInt32(100000, 1000000);
         user.password_reset_token = code.ToString();
         user.password_reset_expires_at = DateTime.UtcNow.AddMinutes(15);

         await _userRepository.UpdateUser(user);

         // Send email with the numeric code for the desktop app using template
         var templateFile = Path.Combine(_templatesPath, "PasswordRecoveryRequest.html");
         string emailBody;
         if (File.Exists(templateFile))
         {
            emailBody = File.ReadAllText(templateFile)
               .Replace("{{USER}}", user.username)
               .Replace("{{CODE}}", user.password_reset_token)
               .Replace("{{EXPIRY_MINUTES}}", "15");
         }
         else
         {
            emailBody = $"Your recovery code is: {user.password_reset_token}. It expires in 15 minutes.";
         }

         _ = Task.Run(async () =>
         {
            try
            {
               await _emailService.SendEmailAsync(user.email, "Password Recovery Code", emailBody);
            }
            catch (Exception ex)
            {
               Console.WriteLine($"Failed to send password recovery email to {user.email}: {ex.Message}");
            }
         });

         return Result<bool>.Ok(true);
      }

      public async Task<Result<bool>> CompletePasswordReset(string token, string newPassword)
      {
         if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(newPassword))
            return Result<bool>.Fail("Token and new password are required");

         var user = await _userRepository.FindByPasswordResetToken(token);
         if (user == null)
            return Result<bool>.Fail("Invalid or expired token");

         // Update password and clear token
         user.password = BCrypt.Net.BCrypt.HashPassword(newPassword.Trim());
         user.password_reset_token = null;
         user.password_reset_expires_at = null;

         await _userRepository.UpdateUser(user);

         // Send an email informing you of the password change using template
         var templateFile = Path.Combine(_templatesPath, "PasswordRecoveryCompleted.html");
         string emailBody;
         if (File.Exists(templateFile))
         {
            emailBody = File.ReadAllText(templateFile)
               .Replace("{{USER}}", user.username);
         }
         else
         {
            emailBody = "Your password has been successfully changed in the TimeTracker application.";
         }

         _ = Task.Run(async () =>
         {
            try
            {
               await _emailService.SendEmailAsync(user.email, "Your password has been successfully changed.", emailBody);
            }
            catch (Exception ex)
            {
               Console.WriteLine($"Failed to send password change email to {user.email}: {ex.Message}");
            }
         });

         return Result<bool>.Ok(true);
      }
   }
}
