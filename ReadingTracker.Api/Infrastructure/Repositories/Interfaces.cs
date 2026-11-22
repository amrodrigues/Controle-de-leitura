using ReadingTracker.Api.Domain.Entities;
using ReadingTracker.Api.Domain.Enums;

namespace ReadingTracker.Api.Infrastructure.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task AddAsync(User user, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}

public interface IBookRepository
{
    Task<List<Book>> GetAllByUserAsync(Guid userId, CancellationToken ct = default);
    Task<Book?> GetByIdAndUserAsync(Guid bookId, Guid userId, CancellationToken ct = default);
    Task<bool> IsInShelfAsync(Guid bookId, Guid userId, CancellationToken ct = default);
    Task AddAsync(Book book, CancellationToken ct = default);
    void Delete(Book book);
    Task SaveChangesAsync(CancellationToken ct = default);
}

public interface IReadingRepository
{
    Task<List<Reading>> GetByUserAsync(Guid userId, ReadingStatus? status, int? year, CancellationToken ct = default);
    Task<Reading?> GetByIdAndUserAsync(Guid readingId, Guid userId, CancellationToken ct = default);
    Task<bool> ExistsBookInShelfAsync(Guid bookId, Guid userId, CancellationToken ct = default);
    Task<int> CountCompletedByUserAndYearAsync(Guid userId, int year, CancellationToken ct = default);
    Task AddAsync(Reading reading, CancellationToken ct = default);
    Task AddReviewAsync(Review review, CancellationToken ct = default);
    void Delete(Reading reading);
    Task SaveChangesAsync(CancellationToken ct = default);
}

public interface IGoalRepository
{
    Task<ReadingGoal?> GetByUserAndYearAsync(Guid userId, int year, CancellationToken ct = default);
    Task AddAsync(ReadingGoal goal, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}