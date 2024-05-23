using AtonTest.Core.DTOs;

namespace AtonTest.Requests;

public class DeleteUserRequest
{
    public required string Login { get; set; }
    public required string Password { get; set; }
    public required DeletionMode DeletionMode { get; set; }
}