using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AtonTest.Core.Interfaces;
using AtonTest.Core.Options;
using AtonTest.Requests;
using AtonTest.Requests.Extentions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AtonTest.Controllers;

[ApiController]
[Route("[controller]")]
public class TokensController : Controller
{
    private readonly IOptionsSnapshot<AtonTestServiceOptions> _optionsSnapshot;
    private readonly IAuthService _authService;

    public TokensController(
        IOptionsSnapshot<AtonTestServiceOptions> optionsSnapshot,
        IAuthService authService)
    {
        _optionsSnapshot = optionsSnapshot;
        _authService = authService;
    }
    
    private string CreateToken(int lifetimeInMinutes, IEnumerable<Claim> claims, string key) {
        var parameters = JwtTokenValidationConfig.GetTokenValidationParameters(key);
        var jwt = new JwtSecurityToken(
            issuer: parameters.ValidIssuer,
            audience: parameters.ValidAudience,
            claims: claims,
            notBefore: DateTime.Now.ToUniversalTime(),
            expires: DateTime.Now.AddMinutes(lifetimeInMinutes).ToUniversalTime(),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                SecurityAlgorithms.HmacSha256
                )
            );     
           
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> GenerateToken(LoginRequest request)
    {
        var dto = request.ToDto();
        bool isAdmin;
        try
        {
             isAdmin = await _authService.ValidateUser(dto);
        }
        catch (ArgumentException e)
        {
            return Unauthorized(e.Message);
        }
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, request.Login),
            new Claim(ClaimTypes.Role, isAdmin ? "Admin" : "User")
        };
        var accessToken = CreateToken(10, claims, _optionsSnapshot.Value.AccessJwtKey!);
        var refreshToken = CreateToken(15, claims, _optionsSnapshot.Value.RefreshJwtKey!);

        return Json(new
        {
            accessToken = accessToken,
            refreshToken = refreshToken
        });
    }

    [HttpGet]
    [Route("refresh")]
    [Authorize(Policy = "Refresh")]
    public IActionResult Refresh()
    {
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var claims = new JwtSecurityTokenHandler().ValidateToken(token,
                JwtTokenValidationConfig.GetTokenValidationParameters(_optionsSnapshot.Value.RefreshJwtKey!), out _).Claims;

        return Json(new
        {
            accessToken = CreateToken(10, claims, _optionsSnapshot.Value.AccessJwtKey!),
            refreshToken = token
        });
    }
}
