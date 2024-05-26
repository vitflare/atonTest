using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AtonTest.Core.Options;
using Microsoft.Extensions.Options;

namespace AtonTest.Middlewares;

public class JwtMiddleware : IMiddleware
{
    private readonly IOptionsSnapshot<AtonTestServiceOptions> _optionsSnapshot;

    public JwtMiddleware(IOptionsSnapshot<AtonTestServiceOptions> optionsSnapshot)
    {
        _optionsSnapshot = optionsSnapshot;
    }
    
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ","");

        if (!String.IsNullOrEmpty(token)) {

            try
            {
                var claims = new JwtSecurityTokenHandler().ReadJwtToken(token).Claims;

                var userLogin = claims.First(x => x.Type is ClaimTypes.Name)?.Value;
                var userRole = claims.First(x => x.Type is ClaimTypes.Role)?.Value;

                context.Items["UserLogin"] = userLogin;
                context.Items["UserRole"] = userRole;
            }
            catch (Exception)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid token");
                return;
            }
        }

        await next(context);
    }
}