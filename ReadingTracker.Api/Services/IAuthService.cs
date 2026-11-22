using ReadingTracker.Api.Domain.Enums;
using ReadingTracker.Api.DTOs;

namespace ReadingTracker.Api.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken ct = default);
}

public interface IBookService
{
    Task<BookResponse> CreateAsync(Guid userId, BookRequest request, CancellationToken ct = default);
    Task<List<BookResponse>> GetAllAsync(Guid userId, CancellationToken ct = default);
    Task<BookResponse> UpdateAsync(Guid userId, Guid bookId, BookRequest request, CancellationToken ct = default);
    Task DeleteAsync(Guid userId, Guid bookId, CancellationToken ct = default);
}

public interface IReadingService
{
    Task<ReadingResponse> AddToShelfAsync(Guid userId, AddReadingRequest request, CancellationToken ct = default);
    Task<List<ReadingResponse>> GetShelfAsync(Guid userId, ReadingStatus? status, int? year, CancellationToken ct = default);
    Task<ReadingResponse> UpdateStatusAsync(Guid userId, Guid readingId, UpdateReadingStatusRequest request, CancellationToken ct = default);
    Task<ReadingResponse> ReReadAsync(Guid userId, Guid bookId, CancellationToken ct = default);
    Task RemoveFromShelfAsync(Guid userId, Guid readingId, CancellationToken ct = default);
    Task<ReviewResponse> UpsertReviewAsync(Guid userId, Guid readingId, UpsertReviewRequest request, CancellationToken ct = default);
}

public interface IGoalService
{
    Task<GoalProgressResponse> CreateAsync(Guid userId, CreateGoalRequest request, CancellationToken ct = default);
    Task<GoalProgressResponse> UpdateAsync(Guid userId, int year, UpdateGoalRequest request, CancellationToken ct = default);
    Task<GoalProgressResponse> GetProgressAsync(Guid userId, int year, CancellationToken ct = default);
}