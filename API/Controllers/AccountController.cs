using System.Security.Claims;
using API.DTOs;
using API.Services;
using Application.Core;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly TokenService _tokenService;
        public AccountController(UserManager<AppUser> userManager, TokenService tokenService)
        {
            _tokenService = tokenService;
            _userManager = userManager;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginDto loginDto)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == loginDto.Email);

            if (user == null)
            {
                return Unauthorized();
            }

            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (result)
            {
                return HandleResult(CreateUserObject(user));
            }

            return Unauthorized();
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterDto registerDto)
        {
            if (await _userManager.Users.AnyAsync(x => x.UserName == registerDto.Username))
            {
                ModelState.AddModelError("username", "Username taken");
                return ValidationProblem();
            }

            if (await _userManager.Users.AnyAsync(x => x.Email == registerDto.Email))
            {
                ModelState.AddModelError("email", "Email taken");
                return ValidationProblem();
            }

            var user = new AppUser
            {
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                UserName = registerDto.Username
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (result.Succeeded)
            {
                return HandleResult(CreateUserObject(user));
            }

            return BadRequest(result.Errors);
        }

        [HttpGet]
        public async Task<ActionResult> GetCurrentUser()
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));

            return HandleResult(CreateUserObject(user));
        }

        private Result<UserDto> CreateUserObject(AppUser user)
        {
            return Result<UserDto>.Success(new UserDto
            {
                DisplayName = user.DisplayName,
                Token = _tokenService.CreateToken(user),
                Username = user.UserName
            });
        }
    }
}
