using System.Reflection;
using AtonTest.Core.DTOs;
using AtonTest.Core.Interfaces;
using AtonTest.Core.Models;
using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;
using pomotracker.Core.Options;

namespace AtonTest.Infrastructure.Database;

public class UserRepository : IUserRepository
{
    private readonly string _connectionString;
    
    public UserRepository(IOptionsSnapshot<DatabaseOptions> options)
    {
        var databaseOptions = options.Value;
        _connectionString = $"host={databaseOptions.Host};port={databaseOptions.Port}" +
                            $";database={databaseOptions.Database};username={databaseOptions.Username};" +
                            $"password={databaseOptions.Password}";
    }

    public async Task CreateUser(CreateUserDto dto)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(
            """
            INSERT INTO users (guid, login, password, name, gender, birthday, is_admin, created_on, created_by, modified_on, modified_by) 
            values (gen_random_uuid(), @login, @password, @name, @gender, @birthday, @is_admin, @created_on, @created_by, @modified_on, @modified_by)
            """);

        var queryParameters = new
        {
            login = dto.Login,
            password = dto.Password,
            name = dto.Name,
            gender = dto.Gender,
            birthday = dto.Birthday,
            is_admin = dto.Admin,
            created_on = DateTime.Now,
            created_by = dto.CreatedBy,
            modified_on = DateTime.Now,
            modified_by = dto.CreatedBy
        };
        
        await connection.QueryFirstOrDefaultAsync(command.CommandText, queryParameters);
    }

    public async Task UpdateUser(User user)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand(
            """
            UPDATE users SET login = @login, password = @password, name = @name, gender = @gender, 
            birthday = @birthday, modified_on = @modified_on, modified_by = @modified_by 
            WHERE guid = @guid
            """);

        var queryParameters = new
        {
            login = user.Login,
            password = user.Password,
            name = user.Name,
            gender = user.Gender,
            birthday = user.Birthday,
            modified_on = user.ModifiedOn,
            modified_by = user.ModifiedBy,
            guid = user.Guid
        };

        await connection.ExecuteAsync(command.CommandText, queryParameters);
    }

    public async Task<int> SoftDeleteUserAsync(DeleteUserDto dto)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        await using var command = new NpgsqlCommand(
            """
            UPDATE users SET revoked_on = @revoked_on, revoked_by = @revoked_by 
            WHERE login = @login AND password = @password
            """, 
            connection);

        var queryParameters = new
        {
            login = dto.Login,
            password = dto.Password,
            revoked_on = DateTime.Now,
            revoked_by = dto.RevokedBy
        };
        var count = await connection.ExecuteAsync(command.CommandText, queryParameters);
        return count;
    }

    public async Task<int> HardDeleteUserAsync(DeleteUserDto dto)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        await using var command = new NpgsqlCommand(
            "DELETE FROM users WHERE login = @login AND password = @password", 
            connection);

        var queryParameters = new
        {
            login = dto.Login,
            password = dto.Password
        };
        
        var count = await connection.ExecuteAsync(command.CommandText, queryParameters);
        return count;
    }

    public async Task<int> RestoreUser(string login)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        await using var command = new NpgsqlCommand(
            "UPDATE users SET revoked_on = NULL, revoked_by = NULL WHERE login = @login AND revoked_on IS NOT NULL", 
            connection);

        var queryParameters = new
        {
            login = login
        };
        
        var count = await connection.ExecuteAsync(command.CommandText, queryParameters);
        return count;
    }
}