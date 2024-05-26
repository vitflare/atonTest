using AtonTest.Core.DTOs;
using AtonTest.Core.Models;

namespace AtonTest.Core.Interfaces;

public interface IUserRepository
{
    Task CreateUser(CreateUserDto dto);
    Task<bool> CheckLogin(string login);
    Task UpdateUser(User user);
    Task<int> SoftDeleteUserAsync(DeleteUserDto dto);
    Task<int> HardDeleteUserAsync(DeleteUserDto dto);
    Task<int> RestoreUser(string login);
}