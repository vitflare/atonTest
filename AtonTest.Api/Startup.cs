using System.Text;
using AtonTest.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using pomotracker.Api;

namespace AtonTest;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddServices();
        services.AddRepositories();
        services.AddScoped<JwtMiddleware>();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,

                    },
                    new List<string>()
                }
            });
        });
        
        services.AddCors();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(ConfigureOptionsAccess)
            .AddJwtBearer("Refresh", ConfigureOptionsRefresh);
        
        services.AddAuthorization(options =>
        { 
            options.AddPolicy("Refresh", new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddAuthenticationSchemes("Refresh")
                .Build());
        });
        services.AddOptions(_configuration);
    }
    
    public void Configure(IHostEnvironment environment, IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        
        app.UseRouting();
        app.UseCors(x => x
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowAnyOrigin());
        
        app.UseHttpsRedirection();

        app.UseMiddleware<JwtMiddleware>();
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
    
    void ConfigureOptionsAccess(JwtBearerOptions jwtBearerOptions)
    {
        jwtBearerOptions.RequireHttpsMetadata = false;
        jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters()
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
                    Encoding.UTF8.GetBytes(_configuration["AtonTestServiceOptions:AccessJwtKey"]!))
            },
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero
        };
    }
    
    void ConfigureOptionsRefresh(JwtBearerOptions jwtBearerOptions)
    {
        jwtBearerOptions.RequireHttpsMetadata = false;
        jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters()
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
                    Encoding.UTF8.GetBytes(_configuration["AtonTestServiceOptions:RefreshJwtKey"]!))
            },
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero
        };
    }
}