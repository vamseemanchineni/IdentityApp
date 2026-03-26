using API.Utility;
using IdentityApp.DTOs;
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
    public class AccountController : ApiCoreController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
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
                return Unauthorized(new ApiResponse(401));
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
                return Unauthorized(new ApiResponse(401, message: "Invalid username or password."));
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, true);
            if(!result.Succeeded)
            {
                RemoveJwtCookie();
                if(result.IsLockedOut)
                {
                    return Unauthorized(new ApiResponse(401, title: "Account locked", 
                        message: StaticDetails.AccountLockedMessage(user.LockoutEnd.Value.DateTime), isHtmlEnabled: true, displayByDefault: true));
                }
                return Unauthorized(new ApiResponse(401, message: "Invalid username or password."));
            }
            return CreateAppUserDto(user);
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if(await checkEmailExistsAsync(model.Email))
            {
                return BadRequest(new ApiResponse(400,message: "Email already exists."));
            }
            if (await checkNameExistsAsync(model.Name))
            {
                return BadRequest(new ApiResponse(400,message: "Username already exists."));
            }
            var userToAdd = new AppUser
            {
                Name = model.Name,
                UserName = model.Name.ToLower(),
                Email = model.Email.ToLower(),
                EmailConfirmed = true,
                LockoutEnabled = true,
            };
             var result = await _userManager.CreateAsync(userToAdd, model.Password);
            if(!result.Succeeded) return BadRequest(result.Errors);
            // Registration logic here
            return Ok(new ApiResponse(200,message: "User registered successfully."));
        }
        [HttpGet("name-taken")]
        public async Task<IActionResult> NameTaken([FromQuery] string name)
        {
            return Ok(new { IsTaken = await checkNameExistsAsync(name) });
        }
        [HttpGet("email-taken")]
        public async Task<IActionResult> EmailTaken([FromQuery] string email)
        {
            return Ok(new { IsTaken = await checkEmailExistsAsync(email) });
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
                Name = user.Name,
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
                Expires = DateTime.UtcNow.AddDays(int.Parse(Configuration["JWT:ExpiresInDays"])),
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

        private async Task<bool> checkNameExistsAsync(string name)
        {
            return await _userManager.Users.AnyAsync(x => x.UserName == name.ToLower());
        }
        #endregion
    }
}
