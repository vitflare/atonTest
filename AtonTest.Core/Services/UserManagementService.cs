using AtonTest.Core.DTOs;
using AtonTest.Core.Interfaces;
using AtonTest.Core.Models;

namespace AtonTest.Core;

public class UserManagementService : IUserManagementService
{
    private readonly IUserRepository _userRepository;

    public UserManagementService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task CreateUser(CreateUserDto dto)
    {
        if (string.IsNullOrEmpty(dto.Login) || string.IsNullOrEmpty(dto.Password) || string.IsNullOrEmpty(dto.Name))
        {
            throw new ArgumentException("Login, Password and Name are required");
        }
        if (! await _userRepository.CheckLogin(dto.Login))
        {
            throw new ArgumentException("Login already exists");
        }
        if (dto.Gender < 0 || dto.Gender > 2)
        {
            throw new ArgumentException("Incorrect gender");
        }
        if (!IsValidString(dto.Login) || !IsValidString(dto.Password) || !IsValidString(dto.Name))
        {
            throw new ArgumentException("Login, Password and Name must contain only letters and numbers");
        }
        if (dto.Birthday > DateTime.Now)
        {
            throw new ArgumentException("Incorrect birthday");
        }
        
        await _userRepository.CreateUser(dto);
    }

    public async Task<int> UpdateUserInfo(UpdateUserInfoDto dto)
    {
        if (string.IsNullOrEmpty(dto.Login))
        {
            throw new ArgumentException("Login is required");
        }
        var user = await _userRepository.GetUserByLogin(dto.Login);
        if (user == null)
        {
            throw new ArgumentException("User not found");
        }
        if (user.RevokedOn != null)
        {
            throw new ArgumentException("User is revoked");
        }
        if (!string.IsNullOrEmpty(dto.Name) && !IsValidString(dto.Name))
        {
            throw new ArgumentException("Name must contain only letters and numbers");
        }
        if (dto.Gender != null && (dto.Gender < 0 || dto.Gender > 2))
        {
            throw new ArgumentException("Incorrect gender");
        }
        if (dto.Birthday != null && dto.Birthday > DateTime.Now)
        {
            throw new ArgumentException("Incorrect birthday");
        }

        var changedFields = 0;
        changedFields += dto.Birthday != null && dto.Birthday != user.Birthday ? 1 : 0;
        user.Birthday = dto.Birthday ?? user.Birthday;
        changedFields += dto.Name != null && dto.Name != user.Name ? 1 : 0;
        user.Name = dto.Name ?? user.Name;
        changedFields += dto.Gender != null && dto.Gender != user.Gender ? 1 : 0;
        user.Gender = dto.Gender ?? user.Gender;
        
        if (changedFields == 0)
        {
            return changedFields;
        }
        user.ModifiedBy = dto.ModifiedBy;
        user.ModifiedOn = DateTime.Now;
        
        await _userRepository.UpdateUser(user);
        return changedFields;
    }

    public async Task UpdateUserPassword(UpdateUserPasswordDto dto)
    {
        if (string.IsNullOrEmpty(dto.Login) || string.IsNullOrEmpty(dto.OldPassword) || string.IsNullOrEmpty(dto.NewPassword))
        {
            throw new ArgumentException("Login and passwords are required");
        }
        var user = await _userRepository.GetUser(dto.Login, dto.OldPassword);
        if (user == null)
        {
            throw new ArgumentException("User not found");
        }
        if (user.RevokedOn != null)
        {
            throw new ArgumentException("User is revoked");
        }
        if (dto.NewPassword.Equals(dto.OldPassword))
        {
            throw new ArgumentException("New password must be different from the old one");
        }
        if (!IsValidString(dto.NewPassword))
        {
            throw new ArgumentException("Password must contain only letters and numbers");
        }
        
        user.Password = dto.NewPassword;
        user.ModifiedBy = dto.ModifiedBy;
        user.ModifiedOn = DateTime.Now;
        
        await _userRepository.UpdateUser(user);
    }

    public async Task UpdateUserLogin(UpdateUserLoginDto dto)
    {
        if (string.IsNullOrEmpty(dto.Login) || string.IsNullOrEmpty(dto.NewLogin))
        {
            throw new ArgumentException("Old and new logins are required");
        }
        var user = await _userRepository.GetUserByLogin(dto.Login);
        if (user == null)
        {
            throw new ArgumentException("User not found");
        }
        if (user.RevokedOn != null)
        {
            throw new ArgumentException("User is revoked");
        }
        if (dto.NewLogin.Equals(dto.Login))
        {
            throw new ArgumentException("New login must be different from the old one");
        }
        if (! await _userRepository.CheckLogin(dto.NewLogin))
        {
            throw new ArgumentException("Login already exists");
        }
        if (!IsValidString(dto.NewLogin))
        {
            throw new ArgumentException("New login must contain only letters and numbers");
        }

        user.Login = dto.NewLogin;
        user.ModifiedBy = dto.ModifiedBy;
        user.ModifiedOn = DateTime.Now;
        
        await _userRepository.UpdateUser(user);
    }

    public async Task DeleteUser(DeleteUserDto dto)
    {
        if (string.IsNullOrEmpty(dto.Login))
        {
            throw new ArgumentException("Login is required");
        }
        int count;
        if (dto.DeletionMode == DeletionMode.Soft)
        {
            count = await _userRepository.SoftDeleteUserAsync(dto);
        }
        else
        {
            count = await _userRepository.HardDeleteUserAsync(dto);
        }
        if (count == 0)
        {
            throw new ArgumentException("User not found");
        }
    }

    public async Task RestoreUser(string login)
    {
        if (string.IsNullOrEmpty(login))
        {
            throw new ArgumentException("Login is required");
        }
        var count = await _userRepository.RestoreUser(login);
        if (count == 0)
        {
            throw new ArgumentException("Revoked user not found");
        }
    }
    
    private bool IsValidString(string input)
    {
        foreach (char c in input)
        {
            if (!((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9')))
            {
                return false;
            }
        }
        return true;
    }
}