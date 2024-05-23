namespace AtonTest.Requests;

public class UpdateUserInfoRequest
{
    public required string Login { get; set; }
    public string? Name { get; set; }
    public int? Gender { get; set; }
    public DateTime? Birthday { get; set; }
}