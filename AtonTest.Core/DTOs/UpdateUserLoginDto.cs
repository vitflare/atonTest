using System.Text.RegularExpressions;

namespace AtonTest.Core.DTOs;

public class UpdateUserLoginDto
{
    public string Login { get; set; }
    public string NewLogin { get; set; }
    public string ModifiedBy { get; set; }

    public void Validate()
    {
        if (string.IsNullOrEmpty(Login) || string.IsNullOrEmpty(NewLogin))
        {
            throw new ArgumentException("Old and new logins are required");
        }
        if (NewLogin.Equals(Login))
        {
            throw new ArgumentException("New login must be different from the old one");
        }
        if (!IsValidString(NewLogin))
        {
            throw new ArgumentException("New login must contain only letters and numbers");
        }
    }
    
    private bool IsValidString(string input)
    {
        var regex = new Regex(@"^[a-zA-Z0-9]+$");
        return regex.IsMatch(input);
    }
}