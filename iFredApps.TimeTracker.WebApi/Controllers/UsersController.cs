using iFredApps.TimeTracker.Core;
using iFredApps.TimeTracker.Core.Interfaces.Services;
using iFredApps.TimeTracker.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace iFredApps.TimeTracker.WebApi.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   public class UsersController : ControllerBase
   {
      private readonly IUserService _userService;
      private readonly string _key;
      private readonly string _issuer;
      private readonly string _audience;

      public UsersController(IUserService userService, IConfiguration configuration)
      {
         _userService = userService;

         _key = configuration["Jwt:Key"];
         _issuer = configuration["Jwt:Issuer"];
         _audience = configuration["Jwt:Audience"];
      }

      // GET: api/Users
      [HttpGet]
      [Authorize]
      public async Task<ActionResult<IEnumerable<User>>> GetUsers()
      {
         var users = await _userService.GetAllUsers();

         //if (users == null || !users.Data.Any())
         //{
         //   return NotFound("No users found.");
         //}

         return Ok(users);
      }

      // GET: api/Users/{id}
      [HttpGet("{id}")]
      [Authorize]
      public async Task<ActionResult<User>> GetUser(int id)
      {
         var user = await _userService.GetUser(id);

         if (user == null)
         {
            return NotFound($"User with ID {id} not found.");
         }

         return Ok(user);
      }

      // POST: api/Users/Login
      [HttpPost("Login")]
      public async Task<ActionResult> Login([FromBody] LoginModel model)
      {
         //if (!ModelState.IsValid)
         //{
         //   return BadRequest("Invalid login data.");
         //}

         var result = await _userService.ValidateUser(model.UserSearchTerm, model.Password);
         if (result == null || !result.Success)
         {
            return Unauthorized(result);
         }

         Result<UserLoginResponse> response = new Result<UserLoginResponse>();
         if (result.Data != null)
         {
            response.Data = new UserLoginResponse
            {
               user_id = result.Data.user_id,
               username = result.Data.username,
               name = result.Data.name,
               email = result.Data.email,
               password = result.Data.password,
               created_at = result.Data.created_at,
            };
         }

         // Generate JWT token
         response.Data.token = GenerateJwtToken(result.Data);

         return Ok(result);
      }

      // POST: api/Users/SignUp
      [HttpPost("SignUp")]
      public async Task<IActionResult> SignUp([FromBody] UserSignUp signUpData)
      {
         if (!ModelState.IsValid)
         {
            return BadRequest("Invalid user data.");
         }

         User user = new User
         {
            name = signUpData.name,
            username = signUpData.username,
            email = signUpData.email,
            password = signUpData.email
         };

         var result = await _userService.CreateUser(user);

         if (!result.Success)
         {
            return BadRequest(new { Errors = result.Errors });
         }

         return CreatedAtAction(nameof(GetUser), new { id = result.Data.user_id }, result);
      }

      // Método privado para gerar o token JWT
      private string GenerateJwtToken(User user)
      {
         var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_key));
         var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

         var claims = new[]
         {
            new Claim(JwtRegisteredClaimNames.Sub, user.username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.user_id.ToString())
        };

         var token = new JwtSecurityToken(
             issuer: _issuer,
             audience: _audience,
             claims: claims,
             expires: DateTime.Now.AddHours(1),
             signingCredentials: credentials
         );

         return new JwtSecurityTokenHandler().WriteToken(token);
      }

      // Classe de modelo para o login
      public class LoginModel
      {
         public string UserSearchTerm { get; set; }
         public string Password { get; set; }
      }
   }

}
