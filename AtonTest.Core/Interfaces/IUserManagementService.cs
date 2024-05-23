using AtonTest.Core.DTOs;
using AtonTest.Core.Models;

namespace AtonTest.Core.Interfaces;

public interface IUserManagementService
{
    Task CreateUser(CreateUserDto dto);
    Task<int> UpdateUserInfo(UpdateUserInfoDto dto);
    Task UpdateUserPassword(UpdateUserPasswordDto dto);
    Task UpdateUserLogin(UpdateUserLoginDto dto);
    Task DeleteUser(DeleteUserDto dto);
    Task RestoreUser(string login);
}