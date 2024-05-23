namespace AtonTest.Requests;

public class CreateUserRequest
{
    public required string Login { get; set; }
    public required string Password { get; set; }
    public required string Name { get; set; }
    public required int Gender { get; set; }
    public required DateTime Birthday { get; set; }
    public required bool Admin { get; set; }
}