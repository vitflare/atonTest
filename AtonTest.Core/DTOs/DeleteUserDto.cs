namespace AtonTest.Core.DTOs;

public class DeleteUserDto
{
    public string Login { get; set; }
    public string Password { get; set; }
    public string RevokedBy { get; set; }
    public DeletionMode DeletionMode { get; set; }
}

public enum DeletionMode
{
    Soft,
    Hard
}