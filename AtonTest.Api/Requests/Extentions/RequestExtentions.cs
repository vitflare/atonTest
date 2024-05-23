using AtonTest.Core.DTOs;

namespace AtonTest.Requests.Extentions;

public static class RequestExtentions
{
    public static LoginDto ToDto(this LoginRequest request)
    {
        return new LoginDto
        {
            Login = request.Login,
            Password = request.Password
        };
    }
}