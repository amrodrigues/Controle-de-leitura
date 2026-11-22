using System.ComponentModel.DataAnnotations;

namespace ReadingTracker.Api.DTOs;

public record RegisterRequest(
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(120, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 120 caracteres.")]
    string Name,

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    string Email,

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [MinLength(8, ErrorMessage = "A senha deve ter pelo menos 8 caracteres.")]
    string Password
);

public record LoginRequest(
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    string Email,

    [Required(ErrorMessage = "A senha é obrigatória.")]
    string Password
);

public record UserResponse(Guid Id, string Name, string Email);

public record AuthResponse(string Token, DateTimeOffset ExpiresAt, UserResponse User);