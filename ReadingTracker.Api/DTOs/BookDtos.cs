using System.ComponentModel.DataAnnotations;

namespace ReadingTracker.Api.DTOs;

public record BookRequest(
    [Required(ErrorMessage = "O título é obrigatório.")]
    [StringLength(120, MinimumLength = 2, ErrorMessage = "O título deve ter entre 2 e 120 caracteres.")]
    string Title,

    [Required(ErrorMessage = "A autora é obrigatória.")]
    [StringLength(120, MinimumLength = 2, ErrorMessage = "A autora deve ter entre 2 e 120 caracteres.")]
    string Author,

    [Range(1, int.MaxValue, ErrorMessage = "O número de páginas deve ser maior que zero.")]
    int PageCount
);

public record BookResponse(Guid Id, string Title, string Author, int PageCount);