using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TimeTracker.Core.Interfaces.Services;
using TimeTracker.Core.Models;

namespace TimerTracker.WebApi.Controllers
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

      [HttpGet("GetUsers")]
      [Authorize]
      public async Task<ActionResult<IEnumerable<User>>> GetUsers()
      {
         var users = await _userService.GetAllUsers();
         return Ok(users);
      }

      [HttpPost("Login")]
      public async Task<ActionResult<User>> Login([FromBody] LoginModel model)
      {
         var userData = await _userService.ValidateUser(model.UserSearchTerm, model.Password);
         if (userData == null)
         {
            return Unauthorized();
         }

         //var tokenHandler = new JwtSecurityTokenHandler();
         //var key = Encoding.ASCII.GetBytes(_key);
         //
         //var tokenDescriptor = new SecurityTokenDescriptor
         //{
         //   Subject = new ClaimsIdentity(new Claim[]
         //   {
         //      new Claim(ClaimTypes.Name, userData.username)
         //   }),
         //   Expires = DateTime.UtcNow.AddHours(1),
         //   SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
         //};
         //
         //var token = tokenHandler.CreateToken(tokenDescriptor);
         //var tokenString = tokenHandler.WriteToken(token);

         return Ok(userData);
      }

      //TODO: Implement auth
      [HttpPost("CreateUser")]
      public async Task<ActionResult> PostUser([FromBody] User user)
      {
         if (user == null || !ModelState.IsValid)
         {
            return BadRequest("Invalid user data.");
         }

         //TODO: Validate if exists
         //var existingUser = await _userService.GetUserByUsername(user.username);
         //if (existingUser != null)
         //{
         //   return Conflict("User already exists.");
         //}

         //var existingEmail = await _userService.GetUserByEmail(user.email);
         //if (existingEmail != null)
         //{
         //   return Conflict("Email already exists.");
         //}

         await _userService.CreateUser(user);

         return Ok("User created successfully.");
      }

      //TODO: Update user info
      //[HttpGet("{id}")]
      //[Authorize]
      //public async Task<ActionResult<User>> GetUser(int id)
      //{
      //   var user = await _userService.GetUserById(id);
      //   if (user == null)
      //   {
      //      return NotFound();
      //   }
      //   return Ok(user);
      //}

      public class LoginModel
      {
         public string UserSearchTerm { get; set; }
         public string Password { get; set; }
      }
   }
}
