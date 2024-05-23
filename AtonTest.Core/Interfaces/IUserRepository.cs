using AtonTest.Core.DTOs;
using AtonTest.Core.Models;

namespace AtonTest.Core.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUser(string login, string password);
    Task CreateUser(CreateUserDto dto);
    Task<bool> CheckLogin(string login);
    Task UpdateUser(User user);
    Task<IEnumerable<User>> GetOnlineUsers();
    Task<User?> GetUserByLogin(string login);
    Task<IEnumerable<User>> GetUsersOlderThan(int age);
    Task<int> SoftDeleteUserAsync(DeleteUserDto dto);
    Task<int> HardDeleteUserAsync(DeleteUserDto dto);
    Task<int> RestoreUser(string login);
}