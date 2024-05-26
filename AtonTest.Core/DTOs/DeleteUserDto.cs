namespace AtonTest.Core.DTOs;

public class DeleteUserDto
{
    public string Login { get; set; }
    public string Password { get; set; }
    public string RevokedBy { get; set; }
    public DeletionMode DeletionMode { get; set; }

    public void Validate()
    {
        if (string.IsNullOrEmpty(Login))
        {
            throw new ArgumentException("Login is required");
        }
    }
}

public enum DeletionMode
{
    Soft,
    Hard
}