using Microsoft.EntityFrameworkCore;
using ReadingTracker.Api.Domain.Entities;
using ReadingTracker.Api.Domain.Enums;
using ReadingTracker.Api.Infrastructure.Data;

namespace ReadingTracker.Api.Infrastructure.Repositories;

public class UserRepository(AppDbContext db) : IUserRepository
{
    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);

    public async Task AddAsync(User user, CancellationToken ct = default) =>
        await db.Users.AddAsync(user, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) =>
        db.SaveChangesAsync(ct);
}

public class BookRepository(AppDbContext db) : IBookRepository
{
    public Task<List<Book>> GetAllByUserAsync(Guid userId, CancellationToken ct = default) =>
        db.Books.AsNoTracking()
            .Where(b => b.UserId == userId)
            .OrderBy(b => b.Title)
            .ToListAsync(ct);

    public Task<Book?> GetByIdAndUserAsync(Guid bookId, Guid userId, CancellationToken ct = default) =>
        db.Books.FirstOrDefaultAsync(b => b.Id == bookId && b.UserId == userId, ct);

    public Task<bool> IsInShelfAsync(Guid bookId, Guid userId, CancellationToken ct = default) =>
        db.Readings.AnyAsync(r => r.BookId == bookId && r.UserId == userId, ct);

    public async Task AddAsync(Book book, CancellationToken ct = default) =>
        await db.Books.AddAsync(book, ct);

    public void Delete(Book book) => db.Books.Remove(book);

    public Task SaveChangesAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);
}

public class ReadingRepository(AppDbContext db) : IReadingRepository
{
    public Task<List<Reading>> GetByUserAsync(Guid userId, ReadingStatus? status, int? year, CancellationToken ct = default)
    {
        var query = db.Readings
            .AsNoTracking()
            .Include(r => r.Book)
            .Include(r => r.Review)
            .Where(r => r.UserId == userId);

        if (status.HasValue)
            query = query.Where(r => r.Status == status.Value);

        if (year.HasValue)
            query = query.Where(r => r.Status == ReadingStatus.Lido && r.FinishedAt.HasValue && r.FinishedAt.Value.Year == year.Value);

        return query.OrderByDescending(r => r.FinishedAt).ThenBy(r => r.Book.Title).ToListAsync(ct);
    }

    public Task<Reading?> GetByIdAndUserAsync(Guid readingId, Guid userId, CancellationToken ct = default) =>
        db.Readings
            .Include(r => r.Book)
            .Include(r => r.Review)
            .FirstOrDefaultAsync(r => r.Id == readingId && r.UserId == userId, ct);

    public Task<bool> ExistsBookInShelfAsync(Guid bookId, Guid userId, CancellationToken ct = default) =>
        db.Readings.AnyAsync(r => r.BookId == bookId && r.UserId == userId, ct);

    public Task<int> CountCompletedByUserAndYearAsync(Guid userId, int year, CancellationToken ct = default) =>
        db.Readings.CountAsync(
            r => r.UserId == userId
              && r.Status == ReadingStatus.Lido
              && r.FinishedAt.HasValue
              && r.FinishedAt.Value.Year == year,
            ct);

    public async Task AddAsync(Reading reading, CancellationToken ct = default) =>
        await db.Readings.AddAsync(reading, ct);

    public async Task AddReviewAsync(Review review, CancellationToken ct = default) =>
        await db.Reviews.AddAsync(review, ct);

    public void Delete(Reading reading) => db.Readings.Remove(reading);

    public Task SaveChangesAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);
}

public class GoalRepository(AppDbContext db) : IGoalRepository
{
    public Task<ReadingGoal?> GetByUserAndYearAsync(Guid userId, int year, CancellationToken ct = default) =>
        db.ReadingGoals.FirstOrDefaultAsync(g => g.UserId == userId && g.Year == year, ct);

    public async Task AddAsync(ReadingGoal goal, CancellationToken ct = default) =>
        await db.ReadingGoals.AddAsync(goal, ct);

    public Task SaveChangesAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);
}