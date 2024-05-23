using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AtonTest.Core.Interfaces;
using AtonTest.Core.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AtonTest.Controllers;

[ApiController]
[Route("[controller]")]
public class UserReadController : ControllerBase
{
    private readonly TokenValidationParameters _validationParameters;
    private readonly IUserRepository _userRepository;
    
    public UserReadController(IOptionsSnapshot<AtonTestServiceOptions> optionsSnapshot, IUserRepository userRepository)
    {
        _userRepository = userRepository;
        _validationParameters = optionsSnapshot.GetTokenValidationParameters();
    }
    
    [HttpGet]
    [Route("get-active-users")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetOnlineUser()
    {
        var users = await _userRepository.GetOnlineUsers();
        var sortedUsers = users.OrderBy(u => u.CreatedOn);
        return Ok(sortedUsers);
    }
    
    [HttpGet]
    [Route("get-by-login")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUserByLogin(string login)
    {
        var user = await _userRepository.GetUserByLogin(login);
        if (user == null)
        {
            return BadRequest("User not found");
        }
        var returnData = new
        {
            user.Name,
            user.Gender,
            user.Birthday,
            Status = user.RevokedOn == null? "Active" : "Revoked",
        };
        return Ok(returnData);
    }
    
    [HttpGet]
    [Route("get-by-login-and-password")]
    [Authorize]
    public async Task<IActionResult> GetUsersByLoginAndPassword(string login, string password)
    {
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var claims = new JwtSecurityTokenHandler().ValidateToken(token, _validationParameters, out _).Claims;
        var loginClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
        if (!loginClaim.Equals(login))
        {
            return Unauthorized("You don't have permission to get information about this user");
        }

        var user = await _userRepository.GetUser(login, password);
        if (user == null)
        {
            return BadRequest("User not found");
        }
        if (user.RevokedOn != null)
        {
            return BadRequest("User revoked");
        }
        return Ok(user);
    }
    
    [HttpGet]
    [Route("get-older-than")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUsersOlderThan(int age)
    {
        if (age < 0)
        {
            return BadRequest("Invalid age");
        }
        var users = await _userRepository.GetUsersOlderThan(age);
        return Ok(users);
    }
}