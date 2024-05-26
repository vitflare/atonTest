using System.Text.RegularExpressions;

namespace AtonTest.Core.DTOs;

public class CreateUserDto
{
    public string Login { get; set; }
    public string Password { get; set; }
    public string Name { get; set; }
    public int Gender { get; set; }
    public DateTime Birthday { get; set; }
    public bool Admin { get; set; }
    public string CreatedBy { get; set; }

    public void Validate()
    {
        if (string.IsNullOrEmpty(Login) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(Name))
        {
            throw new ArgumentException("Login, Password and Name are required");
        }
        if (Gender < 0 || Gender > 2)
        {
            throw new ArgumentException("Incorrect gender");
        }
        if (!IsValidString(Login) || !IsValidString(Password) || !IsValidString(Name))
        {
            throw new ArgumentException("Login, Password and Name must contain only letters and numbers");
        }
        if (Birthday > DateTime.Now)
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