using System.Reflection;
using AtonTest.Core.Interfaces;
using AtonTest.Core.Models;
using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;
using pomotracker.Core.Options;

namespace AtonTest.Infrastructure.Database;

public class ReadonlyUserRepository : IReadonlyUserRepository
{
    private readonly string _connectionString;
    
    public ReadonlyUserRepository(IOptionsSnapshot<DatabaseOptions> options)
    {
        var databaseOptions = options.Value;
        _connectionString = $"host={databaseOptions.Host};port={databaseOptions.Port}" +
                            $";database={databaseOptions.Database};username={databaseOptions.Username};" +
                            $"password={databaseOptions.Password}";
    }
    
    public async Task<User?> GetUser(string login, string password)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        await using var command = new NpgsqlCommand(
            "SELECT * FROM users WHERE login = @login AND password = @password", 
            connection);

        var queryParameters = new
        {
            login = login,
            password = password
        };
        
        var user = await connection.QuerySingleOrDefaultAsync<User?>(command.CommandText, queryParameters);
        return user;
    }

    public async Task<IEnumerable<User>> GetOnlineUsers()
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        await using var command = new NpgsqlCommand("SELECT * FROM users WHERE revoked_on IS NULL", connection);
        var users = await connection.QueryAsync<User>(command.CommandText);
        return users;
    }

    public async Task<User?> GetUserByLogin(string login)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var command = new NpgsqlCommand("SELECT * FROM users WHERE login = @login", connection);
        
        var queryParameters = new
        {
            login = login
        };
        
        var user = await connection.QueryFirstOrDefaultAsync<User>(command.CommandText, queryParameters);
        return user;
    }

    public async Task<IEnumerable<User>> GetUsersOlderThan(int age)
    {
        var dt = DateTime.Now.AddYears(-age);
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        await using var command = new NpgsqlCommand($"SELECT * FROM users WHERE birthday <= @date", connection);
        var queryParameters = new
        {
            date = dt
        };
        var users = await connection.QueryAsync<User>(command.CommandText, queryParameters);
        return users;
    }
}