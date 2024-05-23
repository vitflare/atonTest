using AtonTest.Core.DTOs;
using AtonTest.Core.Interfaces;

namespace AtonTest.Core;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<bool> ValidateUser(LoginDto dto)
    {
        var user = await _userRepository.GetUser(dto.Login, dto.Password);
        if (user == null || user.RevokedOn != null)
        {
            throw new ArgumentException("User not found");
        }

        return user.Admin;
    }
    
}