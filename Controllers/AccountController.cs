using API.Utility;
using IdentityApp.DTOs.Account;
using IdentityApp.Extensions;
using IdentityApp.Models;
using IdentityApp.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace IdentityApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _config;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService, IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _config = config;
        }
        [HttpGet("auth-status")]
        public IActionResult IsLoggedIn()
        {
            return Ok(new {IsAuthenticated = User.Identity?.IsAuthenticated ?? false });
        }
        [Authorize]
        [HttpGet("refresh-appuser")]
        public async Task<ActionResult<AppUserDto>> RefreshAppUser()
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x=>x.Id == User.GetUserId());
            if (user == null)
            {
                RemoveJwtCookie();
                return Unauthorized();
            }
            return CreateAppUserDto(user);
        }
        [HttpPost("login")]
        public async Task<ActionResult<AppUserDto>> login(LoginDto model)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == model.Username.ToLower());
            if(user == null)
            {
                user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == model.Username.ToLower());
            }
            if(user == null)
            {
                return Unauthorized("Invalid username or password.");
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if(!result.Succeeded)
            {
                RemoveJwtCookie();
                return Unauthorized("Invalid username or password.");
            }
            return CreateAppUserDto(user);
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if(await checkEmailExistsAsync(model.Email))
            {
                return BadRequest("Email already exists.");
            }
            if (await checkUsernameExistsAsync(model.Username))
            {
                return BadRequest("Username already exists.");
            }
            var userToAdd = new AppUser
            {
                UserName = model.Username.ToLower(),
                Email = model.Email.ToLower(),
                EmailConfirmed = true
            };
             var result = await _userManager.CreateAsync(userToAdd, model.Password);
            if(!result.Succeeded) return BadRequest(result.Errors);
            // Registration logic here
            return Ok("User registered successfully.");
        }
        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            RemoveJwtCookie();
            return NoContent();
        }

        #region Private Methods
        private AppUserDto CreateAppUserDto(AppUser user)
        {
            string jwt = _tokenService.CreateJWT(user);
            SetJWTCookie(jwt);
            return new AppUserDto
            {
                UserName = user.UserName,
                Jwt = jwt
            };
        }
        private void SetJWTCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                IsEssential = true,
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddDays(int.Parse(_config["JWT:ExpiresInDays"])),
                SameSite = SameSiteMode.None
            };
            Response.Cookies.Append(StaticDetails.IdentityAppCookie, token, cookieOptions);
        }
        private void RemoveJwtCookie()
        {
            Response.Cookies.Delete(StaticDetails.IdentityAppCookie);
        }
        private async Task<bool> checkEmailExistsAsync(string email)
        {
            return await _userManager.Users.AnyAsync(x=>x.Email == email.ToLower());
        }

        private async Task<bool> checkUsernameExistsAsync(string username)
        {
            return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
        #endregion
    }
}
