using AtonTest.Core.DTOs;
using AtonTest.Core.Interfaces;

namespace AtonTest.Core;

public class UserManagementService : IUserManagementService
{
    private readonly IUserRepository _userRepository;
    private readonly IReadonlyUserRepository _readonlyUserRepository;

    public UserManagementService(IUserRepository userRepository,
        IReadonlyUserRepository readonlyUserRepository)
    {
        _userRepository = userRepository;
        _readonlyUserRepository = readonlyUserRepository;
    }
    
    public async Task CreateUser(CreateUserDto dto)
    {
        if (! await _userRepository.CheckLogin(dto.Login))
        {
            throw new ArgumentException("Login already exists");
        }
        dto.Validate();
        await _userRepository.CreateUser(dto);
    }

    public async Task<int> UpdateUserInfo(UpdateUserInfoDto dto)
    {
        dto.Validate();
        var user = await _readonlyUserRepository.GetUserByLogin(dto.Login);
        if (user is null)
            throw new ArgumentException("User not found");
        if (user.RevokedOn is not null)
        {
            throw new ArgumentException("User is revoked");
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
        dto.Validate();
        var user = await _readonlyUserRepository.GetUser(dto.Login, dto.OldPassword);
        if (user is null)
        {
            throw new ArgumentException("User not found");
        }
        if (user.RevokedOn is not null)
        {
            throw new ArgumentException("User is revoked");
        }
        user.Password = dto.NewPassword;
        user.ModifiedBy = dto.ModifiedBy;
        user.ModifiedOn = DateTime.Now;
        
        await _userRepository.UpdateUser(user);
    }

    public async Task UpdateUserLogin(UpdateUserLoginDto dto)
    {
        dto.Validate();
        var user = await _readonlyUserRepository.GetUserByLogin(dto.Login);
        if (user is null)
        {
            throw new ArgumentException("User not found");
        }
        if (user.RevokedOn is not null)
        {
            throw new ArgumentException("User is revoked");
        }
        if (! await _userRepository.CheckLogin(dto.NewLogin))
        {
            throw new ArgumentException("Login already exists");
        }
        user.Login = dto.NewLogin;
        user.ModifiedBy = dto.ModifiedBy;
        user.ModifiedOn = DateTime.Now;
        
        await _userRepository.UpdateUser(user);
    }

    public async Task DeleteUser(DeleteUserDto dto)
    {
        dto.Validate();
        int count;
        if (dto.DeletionMode is DeletionMode.Soft)
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
}