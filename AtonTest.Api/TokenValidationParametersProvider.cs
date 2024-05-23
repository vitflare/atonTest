using AtonTest.Core.Options;

namespace AtonTest;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

public static class JwtTokenValidationConfig
{
    public static TokenValidationParameters GetTokenValidationParameters(this IOptionsSnapshot<AtonTestServiceOptions> optionsSnapshot)
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
                    Encoding.UTF8.GetBytes(optionsSnapshot.Value.JwtKey!))
            },
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero
        };
    }
}
