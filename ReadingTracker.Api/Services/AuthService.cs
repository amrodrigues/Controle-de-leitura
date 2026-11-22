using ReadingTracker.Api.Domain.Entities;
using ReadingTracker.Api.Domain.Exceptions;
using ReadingTracker.Api.DTOs;
using ReadingTracker.Api.Infrastructure.Repositories;
using ReadingTracker.Api.Infrastructure.Security;

namespace ReadingTracker.Api.Services;

public class AuthService(IUserRepository users, IJwtTokenGenerator jwt) : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var existing = await users.GetByEmailAsync(normalizedEmail, ct);
        if (existing is not null)
            throw new ConflictAppException("Já existe uma conta cadastrada com este e-mail.", "Email");

        var user = new User
        {
            Name = request.Name.Trim(),
            Email = normalizedEmail,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        await users.AddAsync(user, ct);
        await users.SaveChangesAsync(ct);

        var (token, expiresAt) = jwt.Generate(user);
        return new AuthResponse(token, expiresAt, new UserResponse(user.Id, user.Name, user.Email));
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await users.GetByEmailAsync(normalizedEmail, ct);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAppException("E-mail ou senha inválidos.");

        var (token, expiresAt) = jwt.Generate(user);
        return new AuthResponse(token, expiresAt, new UserResponse(user.Id, user.Name, user.Email));
    }
}