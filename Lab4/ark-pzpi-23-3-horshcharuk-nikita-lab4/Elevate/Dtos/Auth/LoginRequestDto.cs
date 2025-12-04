namespace Elevate.Dtos.Auth;

public class LoginRequestDto
{
    public string LoginOrEmail { get; set; } = null!;
    public string Password { get; set; } = null!;
}
