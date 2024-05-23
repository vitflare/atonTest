namespace AtonTest.Requests;

public class UpdateUserPasswordRequest
{
    public required string Login { get; set; }
    public required string OldPassword { get; set; }
    public required string NewPassword { get; set; }
}