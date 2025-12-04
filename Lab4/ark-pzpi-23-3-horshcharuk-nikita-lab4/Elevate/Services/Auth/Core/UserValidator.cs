using Elevate.Entities;
using Microsoft.AspNetCore.Identity;

namespace Elevate.Services.Auth.Core;

public class UserValidator
{
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserValidator(IPasswordHasher<User> passwordHasher)
    {
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
    }

    public void ValidatePassword(User user, string password)
    {
        ArgumentNullException.ThrowIfNull(user);
        ArgumentNullException.ThrowIfNull(password);

        var result = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            password);

        if (result == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }
    }
}

