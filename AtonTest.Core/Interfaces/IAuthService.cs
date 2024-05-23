using AtonTest.Core.DTOs;

namespace AtonTest.Core.Interfaces;

public interface IAuthService
{
    Task<bool> ValidateUser(LoginDto dto);
}