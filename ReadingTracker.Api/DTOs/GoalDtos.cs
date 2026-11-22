using System.ComponentModel.DataAnnotations;

namespace ReadingTracker.Api.DTOs;

public record CreateGoalRequest(
    [Range(2000, 2100, ErrorMessage = "Informe um ano válido.")]
    int Year,

    [Range(1, 10000, ErrorMessage = "A meta deve ser maior que zero.")]
    int TargetBooks
);

public record UpdateGoalRequest(
    [Range(1, 10000, ErrorMessage = "A meta deve ser maior que zero.")]
    int TargetBooks
);

public record GoalProgressResponse(
    int Year,
    int TargetBooks,
    int CompletedBooks,
    decimal CompletionPercentage,
    int RemainingBooks,
    int RemainingMonths,
    decimal RequiredMonthlyAverage
);