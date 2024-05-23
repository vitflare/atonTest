namespace AtonTest.Core.DTOs;

public class UpdateUserInfoDto
{
    public string Login { get; set; }
    public string? Name { get; set; }
    public int? Gender { get; set; }
    public DateTime? Birthday { get; set; }
    public string ModifiedBy { get; set; }
}