using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AtonTest.Core.DTOs;
using AtonTest.Core.Interfaces;
using AtonTest.Requests;
using Microsoft.AspNetCore.Authorization;
using AtonTest.Core.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AtonTest.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly TokenValidationParameters _validationParameters;
    private readonly IUserManagementService _userManagementService;
    
    public UsersController(IOptionsSnapshot<AtonTestServiceOptions> optionsSnapshot, IUserManagementService userManagementService)
    {
        _validationParameters = optionsSnapshot.GetTokenValidationParameters();
        _userManagementService = userManagementService;
    }
    
    [HttpPost]
    [Route("create")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateUser(CreateUserRequest request)
    {
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var claims = new JwtSecurityTokenHandler().ValidateToken(token, _validationParameters, out _).Claims;
        
        CreateUserDto dto = new CreateUserDto
        {
            Login = request.Login,
            Password = request.Password,
            Name = request.Name,
            Gender = request.Gender,
            Birthday = request.Birthday,
            Admin = request.Admin,
            CreatedBy = claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value
        };
        try
        {
            await _userManagementService.CreateUser(dto);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }

        return Ok("User created successfully!");
    }

    [HttpPatch]
    [Route("update-info")]
    [Authorize]
    public async Task<IActionResult> UpdateUserInfo(UpdateUserInfoRequest request)
    {
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var claims = new JwtSecurityTokenHandler().ValidateToken(token, _validationParameters, out _).Claims;
        var login = claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
        var role = claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
        if (!role.Equals("Admin") && login != request.Login)
        {
            return Unauthorized("You don't have permission to update this user");
        }

        UpdateUserInfoDto dto = new UpdateUserInfoDto
        {
            Login = request.Login,
            Name = request.Name,
            Birthday = request.Birthday,
            Gender = request.Gender,
            ModifiedBy = login
        };

        int count;
        try
        {
            count = await _userManagementService.UpdateUserInfo(dto);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }

        return count == 0?  Ok("No changes were made") : Ok("User info updated successfully!");
    }

    [HttpPatch]
    [Route("update-login")]
    [Authorize]
    public async Task<IActionResult> UpdateUserLogin(string oldLogin, string newLogin)
    {
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var claims = new JwtSecurityTokenHandler().ValidateToken(token, _validationParameters, out _).Claims;
        var login = claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
        var role = claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
        if (!role.Equals("Admin") && login != oldLogin)
        {
            return Unauthorized("You don't have permission to update this user");
        }
        
        UpdateUserLoginDto dto = new UpdateUserLoginDto
        {
            Login = oldLogin,
            NewLogin = newLogin,
            ModifiedBy = login
        };
        
        try
        {
            await _userManagementService.UpdateUserLogin(dto);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
        
        return Ok("User login updated successfully!");
    }
    
    [HttpPatch]
    [Route("update-password")]
    [Authorize]
    public async Task<IActionResult> UpdateUserPassword(UpdateUserPasswordRequest request)
    {
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var claims = new JwtSecurityTokenHandler().ValidateToken(token, _validationParameters, out _).Claims;
        var login = claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
        var role = claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
        if (!role.Equals("Admin") && login != request.Login)
        {
            return Unauthorized("You don't have permission to update this user");
        }
        
        UpdateUserPasswordDto dto = new UpdateUserPasswordDto
        {
            Login = request.Login,
            OldPassword = request.OldPassword,
            NewPassword = request.NewPassword,
            ModifiedBy = login
        };
        
        try
        {
            await _userManagementService.UpdateUserPassword(dto);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
        
        return Ok("User password updated successfully!");
    }

    [HttpDelete]
    [Route("delete")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(DeleteUserRequest request)
    {
        if (!Enum.IsDefined(typeof(DeletionMode), request.DeletionMode))
        {
            return BadRequest("Invalid deletion mode");
        }
        
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var claims = new JwtSecurityTokenHandler().ValidateToken(token, _validationParameters, out _).Claims;
        
        if (claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value == request.Login)
        {
            return BadRequest("You can't delete your own account");
        }
        DeleteUserDto dto = new DeleteUserDto
        {
            Login = request.Login,
            Password = request.Password,
            DeletionMode = request.DeletionMode,
            RevokedBy = claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value
        };
        
        try
        {
            await _userManagementService.DeleteUser(dto);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }

        return Ok("User deleted successfully!");
    }
    
    [HttpPut]
    [Route("restore")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RestoreUser(string login)
    {
        try
        {
            await _userManagementService.RestoreUser(login);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
        
        return Ok("User restored successfully!");
    }
}