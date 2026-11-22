using System.ComponentModel.DataAnnotations;
using ReadingTracker.Api.Domain.Enums;

namespace ReadingTracker.Api.DTOs;

public record AddReadingRequest(
    [Required(ErrorMessage = "O identificador do livro é obrigatório.")]
    Guid BookId
);

public record UpdateReadingStatusRequest(
    [Required(ErrorMessage = "O novo status é obrigatório.")]
    ReadingStatus Status,

    DateOnly? StartedAt,
    DateOnly? FinishedAt
);

public record ReadingResponse(
    Guid Id,
    Guid BookId,
    string BookTitle,
    string BookAuthor,
    int PageCount,
    ReadingStatus Status,
    DateOnly? StartedAt,
    DateOnly? FinishedAt,
    ReviewResponse? Review
);