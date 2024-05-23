namespace AtonTest.Core.DTOs;

public class UpdateUserPasswordDto
{
    public string Login { get; set; }
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
    public string ModifiedBy { get; set; }
}