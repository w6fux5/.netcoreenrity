using System.Security.Claims;
using API.Dtos;
using API.Services;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly TokenService _tokenService;

        public AccountController(UserManager<AppUser> userManager, TokenService tokenService)
        {
            _tokenService = tokenService;
            _userManager = userManager;
        }

        [AllowAnonymous] // 可以不用驗證token
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            AppUser user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null)
                return Unauthorized();

            bool result = await _userManager.CheckPasswordAsync(user, loginDto.Password);

            if (result)
                return CreateUserObj(user);

            return Unauthorized();
        }

        [AllowAnonymous] // 可以不用驗證token
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await _userManager.Users.AnyAsync(x => x.UserName == registerDto.Username))
            {
                return BadRequest("Username is already taken");
            }

            AppUser user =
                new()
                {
                    DisplayName = registerDto.DisplayName,
                    Email = registerDto.Email,
                    UserName = registerDto.Username,
                };

            IdentityResult result = await _userManager.CreateAsync(user, registerDto.Password);

            if (result.Succeeded)
            {
                return CreateUserObj(user);
            }

            return BadRequest(result.Errors);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
            return CreateUserObj(user);
        }

        private UserDto CreateUserObj(AppUser user)
        {
            return new UserDto
            {
                DisplayName = user.DisplayName,
                Username = user.UserName,
                Image = null,
                Token = _tokenService.CreateToken(user)
            };
        }
    }
}
