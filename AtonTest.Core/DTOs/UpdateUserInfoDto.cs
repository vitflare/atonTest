using System.Text.RegularExpressions;

namespace AtonTest.Core.DTOs;

public class UpdateUserInfoDto
{
    public string Login { get; set; }
    public string? Name { get; set; }
    public int? Gender { get; set; }
    public DateTime? Birthday { get; set; }
    public string ModifiedBy { get; set; }

    public void Validate()
    {
        if (string.IsNullOrEmpty(Login))
        {
            throw new ArgumentException("Login is required");
        }
        if (!string.IsNullOrEmpty(Name) && !IsValidString(Name))
        {
            throw new ArgumentException("Name must contain only letters and numbers");
        }
        if (Gender != null && (Gender < 0 || Gender > 2))
        {
            throw new ArgumentException("Incorrect gender");
        }
        if (Birthday != null && Birthday > DateTime.Now)
        {
            throw new ArgumentException("Incorrect birthday");
        }
    }
    
    private bool IsValidString(string input)
    {
        var regex = new Regex(@"^[a-zA-Z0-9]+$");
        return regex.IsMatch(input);
    }
}