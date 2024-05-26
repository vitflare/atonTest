using System.Text.RegularExpressions;

namespace AtonTest.Core.DTOs;

public class UpdateUserPasswordDto
{
    public string Login { get; set; }
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
    public string ModifiedBy { get; set; }

    public void Validate()
    {
        if (string.IsNullOrEmpty(Login) || string.IsNullOrEmpty(OldPassword) || string.IsNullOrEmpty(NewPassword))
        {
            throw new ArgumentException("Login and passwords are required");
        }
        if (NewPassword.Equals(OldPassword))
        {
            throw new ArgumentException("New password must be different from the old one");
        }
        if (!IsValidString(NewPassword))
        {
            throw new ArgumentException("Password must contain only letters and numbers");
        }
    }
    
    private bool IsValidString(string input)
    {
        var regex = new Regex(@"^[a-zA-Z0-9]+$");
        return regex.IsMatch(input);
    }
}