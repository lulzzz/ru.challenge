﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using RU.Challenge.Domain.Entities.Auth;
using RU.Challenge.Infrastructure.Identity;
using RU.Challenge.Infrastructure.Identity.DTO;
using RU.Challenge.Infrastructure.Identity.Extensions;
using RU.Challenge.Presentation.API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RU.Challenge.Presentation.API.Controllers
{
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly JwtFactory _jwtFactory;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly SignInManager<User> _signInManager;

        public AuthController(
            JwtFactory jwtFactory,
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            SignInManager<User> signInManager)
        {
            _jwtFactory = jwtFactory ?? throw new ArgumentNullException(nameof(jwtFactory));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterUser([FromBody] CreateUser userModel, ClaimsEnum claim)
        {
            if (!ModelState.IsValid)
                return BadRequest($@"The field(s) {string.Join(", ", ModelState
                    .Where(e => e.Value.ValidationState == ModelValidationState.Invalid).Select(e => e.Key))} are not valid");

            // Use a GUID for Id
            var userId = Guid.NewGuid();

            // Create the user object
            var user = new User
            {
                Id = userId,
                Email = userModel.Email,
                UserName = userModel.Username,
            };

            var userClaims = new[] {
                new Claim("roles", claim.ToString())
            };

            // Add the user
            var result = await _userManager.CreateAsync(user, userModel.Password);

            if (result.Succeeded)
            {
                // Add claims
                await _userManager.AddClaimsAsync(user, userClaims);

                var token = GenerateToken(userId, userClaims, user.UserName);
                return Ok(token);
            }

            return BadRequest(result.Errors);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginUser loginUser)
        {
            if (!ModelState.IsValid)
                return BadRequest($@"The field(s) {string.Join(", ", ModelState
                    .Where(e => e.Value.ValidationState == ModelValidationState.Invalid).Select(e => e.Key))} are not valid");

            var result = await _signInManager.PasswordSignInAsync(loginUser.UserName, loginUser.Password, isPersistent: true, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var authUser = await _userManager.FindByNameAsync(loginUser.UserName);
                var claims = await _userManager.GetClaimsAsync(authUser);

                var token = GenerateToken(authUser.Id, claims, authUser.UserName);
                return Ok(token);
            }
            return BadRequest("Invalid login attempt");
        }

        [HttpPost]
        [Route("refreshtoken")]
        [Authorize(Roles = "DataEntry, ReleaseManager")]
        public IActionResult RefreshToken()
        {
            var roles = User.Claims.GetRoles();
            var userId = User.Claims.GetUserId();
            var userName = User.Claims.GetUserName();

            var token = GenerateToken(userId, roles, userName);
            return Ok(token);
        }

        private TokenResponse GenerateToken(Guid userId, IEnumerable<Claim> roles, string userName)
        {
            var tokenExp = TimeSpan.FromMinutes(10);
            var token = _jwtFactory.GenerateToken(userId.ToString(), userName, roles, tokenDuration: tokenExp);
            return new TokenResponse(userId, token, (long)tokenExp.TotalSeconds);
        }
    }
}