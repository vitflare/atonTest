using AtonTest.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AtonTest.Controllers;

[ApiController]
[Route("[controller]")]
public class UserReadController : ControllerBase
{
    private readonly IReadonlyUserRepository _readonlyUserRepository;
    
    public UserReadController(IReadonlyUserRepository readonlyUserRepository)
    {
        _readonlyUserRepository = readonlyUserRepository;
    }
    
    [HttpGet]
    [Route("get-active-users")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetOnlineUser()
    {
        var users = await _readonlyUserRepository.GetOnlineUsers();
        var sortedUsers = users.OrderBy(u => u.CreatedOn);
        return Ok(sortedUsers);
    }
    
    [HttpGet]
    [Route("get-by-login")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUserByLogin(string login)
    {
        var user = await _readonlyUserRepository.GetUserByLogin(login);
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
        var loginClaim = HttpContext.Items["UserLogin"].ToString();
        if (!loginClaim.Equals(login))
        {
            return Unauthorized("You don't have permission to get information about this user");
        }

        var user = await _readonlyUserRepository.GetUserByLogin(login);
        if (user == null)
        {
            return BadRequest("User not found");
        }
        if (user.Password != password)
        {
            return BadRequest("Invalid password");
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
        var users = await _readonlyUserRepository.GetUsersOlderThan(age);
        return Ok(users);
    }
}