using AtonTest.Core.Models;

namespace AtonTest.Core.Interfaces;

public interface IReadonlyUserRepository
{
    Task<User?> GetUser(string login, string password);
    Task<IEnumerable<User>> GetOnlineUsers();
    Task<User?> GetUserByLogin(string login);
    Task<IEnumerable<User>> GetUsersOlderThan(int age);
}