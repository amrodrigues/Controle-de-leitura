using System.ComponentModel.DataAnnotations;

namespace ReadingTracker.Api.DTOs;

public record UpsertReviewRequest(
    [Range(1, 5, ErrorMessage = "A nota deve ser entre 1 e 5.")]
    int Rating,

    [MaxLength(1000, ErrorMessage = "O comentário deve ter no máximo 1000 caracteres.")]
    string? Comment
);

public record ReviewResponse(Guid Id, Guid ReadingId, int Rating, string? Comment, DateTimeOffset UpdatedAt);