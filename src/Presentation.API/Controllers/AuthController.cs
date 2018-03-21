using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using RU.Challenge.Domain.Entities.Auth;
using RU.Challenge.Infrastructure.Identity;
using System;
using System.Threading.Tasks;
using RU.Challenge.Infrastructure.Identity.Extensions;
using Microsoft.AspNetCore.Authorization;
using RU.Challenge.Infrastructure.Identity.DTO;

namespace RU.Challenge.Presentation.API.Controllers
{
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly JwtFactory _jwtFactory;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthController(
            JwtFactory jwtFactory,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _jwtFactory = jwtFactory;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterUser([FromBody] CreateUser userModel)
        {
            // Use a GUID for Id
            var userId = Guid.NewGuid();

            // Create the user object
            var user = new User
            {
                Id = userId,
                Email = userModel.Email,
                UserName = userModel.Username,
            };

            // Add the user
            var result = await _userManager.CreateAsync(user, userModel.Password);

            if (result.Succeeded)
            {
                var tokenExp = TimeSpan.FromMinutes(10);
                var token = _jwtFactory.GenerateToken(user.Id.ToString(), user.UserName, roles: new[] { "ReleaseManager" }, tokenDuration: tokenExp);
                return Ok(new TokenResponse(userId, token, (long)tokenExp.TotalSeconds));
            }

            return BadRequest(result.Errors);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginUser loginUser)
        {
            var result = await _signInManager.PasswordSignInAsync(loginUser.Email, loginUser.Password, isPersistent: true, lockoutOnFailure: false);
            if (result.Succeeded)
            { 
                //var tokenExp = TimeSpan.FromMinutes(10);
                //var token = _jwtFactory.GenerateToken(user.Id.ToString(), user.UserName, roles: new[] { "ReleaseManager" }, tokenDuration: tokenExp);
                //return Ok(new TokenResponse(userId, token, (long)tokenExp.TotalSeconds));
            }
            return BadRequest("Invalid login attempt");
        }

        [HttpPost]
        [Route("refreshtoken")]
        [Authorize(Roles = "ReleaseManager")]
        public IActionResult RefreshToken()
        {
            var roles = User.Claims.GetRoles();
            var userId = User.Claims.GetUserId();
            var userName = User.Claims.GetUserName();

            var tokenExp = TimeSpan.FromMinutes(10);
            var token = _jwtFactory.GenerateToken(userId.ToString(), userName, roles, tokenDuration: tokenExp);
            return Ok(new TokenResponse(userId, token, (long)tokenExp.TotalSeconds));
        }
    }
}