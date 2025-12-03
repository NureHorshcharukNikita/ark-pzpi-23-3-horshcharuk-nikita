namespace Elevate.Dtos.Auth;

public class RegisterRequestDto
{
    public string Login { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
}
