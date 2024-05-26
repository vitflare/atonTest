using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AtonTest;

public static class JwtTokenValidationConfig
{
    public static TokenValidationParameters GetTokenValidationParameters(string key)
    {
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "AtonTestAuthService",
            ValidateAudience = true,
            ValidAudience = "AtonTestAuthService",
            ValidateLifetime = true,
            IssuerSigningKeyResolver = (string token, SecurityToken securityToken, string kid,
                TokenValidationParameters parameters) => new List<SecurityKey>()
            {
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(key))
            },
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero
        };
    }
}
