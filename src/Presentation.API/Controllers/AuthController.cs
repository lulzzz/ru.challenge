using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using RU.Challenge.Domain.Entities.Auth;
using RU.Challenge.Infrastructure.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<IActionResult> RegisterUser([FromBody] Infrastructure.Identity.DTO.User userModel)
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
                return Ok(new Infrastructure.Identity.DTO.TokenResponse(userId, token, (long)tokenExp.TotalSeconds));
            }
            else
                return BadRequest(result.Errors);
        }

        [HttpPost]
        [Route("refreshtoken")]
        public async Task<IActionResult> RefreshToken()
        {
            var asd = User.Claims;
            return Ok();
        }

    }
}