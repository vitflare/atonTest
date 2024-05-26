using AtonTest.Core.DTOs;
using AtonTest.Core.Interfaces;

namespace AtonTest.Core;

public class AuthService : IAuthService
{
    private readonly IReadonlyUserRepository _readonlyUserRepository;

    public AuthService(IReadonlyUserRepository readonlyUserRepository)
    {
        _readonlyUserRepository = readonlyUserRepository;
    }
    
    public async Task<bool> ValidateUser(LoginDto dto)
    {
        var user = await _readonlyUserRepository.GetUser(dto.Login, dto.Password);
        if (user is null || user.RevokedOn is not null)
        {
            throw new ArgumentException("User not found");
        }

        return user.Admin;
    }
    
}