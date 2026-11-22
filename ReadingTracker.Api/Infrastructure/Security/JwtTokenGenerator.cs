using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ReadingTracker.Api.Domain.Entities;

namespace ReadingTracker.Api.Infrastructure.Security;

public interface IJwtTokenGenerator
{
    (string Token, DateTimeOffset ExpiresAt) Generate(User user);
}

public class JwtTokenGenerator(IConfiguration config) : IJwtTokenGenerator
{
    public (string Token, DateTimeOffset ExpiresAt) Generate(User user)
    {
        var secret = config["Jwt:SecretKey"]
            ?? throw new InvalidOperationException("Jwt:SecretKey não configurada.");
        var issuer = config["Jwt:Issuer"] ?? "ReadingTracker.Api";
        var audience = config["Jwt:Audience"] ?? "ReadingTracker.Client";
        var hours = int.TryParse(config["Jwt:ExpirationHours"], out var h) ? h : 8;

        var expiresAt = DateTimeOffset.UtcNow.AddHours(hours);
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.Name)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt.UtcDateTime,
            signingCredentials: creds
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}