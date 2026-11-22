using ReadingTracker.Api.Domain.Entities;
using ReadingTracker.Api.Domain.Exceptions;
using ReadingTracker.Api.DTOs;
using ReadingTracker.Api.Infrastructure.Repositories;

namespace ReadingTracker.Api.Services;

public class GoalService(
    IGoalRepository goals,
    IReadingRepository readings,
    TimeProvider timeProvider) : IGoalService
{
    public async Task<GoalProgressResponse> CreateAsync(Guid userId, CreateGoalRequest request, CancellationToken ct = default)
    {
        // Regra 7: Uma meta por ano
        var existing = await goals.GetByUserAndYearAsync(userId, request.Year, ct);
        if (existing is not null)
            throw new ConflictAppException($"Já existe uma meta cadastrada para o ano {request.Year}. Utilize a rota de atualização.", "Year");

        var goal = new ReadingGoal
        {
            UserId = userId,
            Year = request.Year,
            TargetBooks = request.TargetBooks
        };

        await goals.AddAsync(goal, ct);
        await goals.SaveChangesAsync(ct);

        return await BuildProgressAsync(userId, goal, ct);
    }

    public async Task<GoalProgressResponse> UpdateAsync(Guid userId, int year, UpdateGoalRequest request, CancellationToken ct = default)
    {
        var goal = await goals.GetByUserAndYearAsync(userId, year, ct)
            ?? throw new NotFoundAppException($"Meta para o ano {year} não encontrada.");

        goal.TargetBooks = request.TargetBooks;
        await goals.SaveChangesAsync(ct);

        return await BuildProgressAsync(userId, goal, ct);
    }

    public async Task<GoalProgressResponse> GetProgressAsync(Guid userId, int year, CancellationToken ct = default)
    {
        var goal = await goals.GetByUserAndYearAsync(userId, year, ct)
            ?? throw new NotFoundAppException($"Meta para o ano {year} não encontrada.");

        return await BuildProgressAsync(userId, goal, ct);
    }

    private async Task<GoalProgressResponse> BuildProgressAsync(Guid userId, ReadingGoal goal, CancellationToken ct)
    {
        // Regra 5 e 6: Lidos com status Lido e data de término no ano
        var completed = await readings.CountCompletedByUserAndYearAsync(userId, goal.Year, ct);
        var percentage = Math.Round((completed / (decimal)goal.TargetBooks) * 100m, 1);
        var remainingBooks = Math.Max(0, goal.TargetBooks - completed);

        var now = timeProvider.GetUtcNow().UtcDateTime;
        int remainingMonths = goal.Year > now.Year
            ? 12
            : goal.Year < now.Year
                ? 0
                : 12 - now.Month + 1; // Conta o mês atual (ex.: outubro = 12 - 10 + 1 = 3)

        decimal monthlyAverage = (remainingBooks == 0 || remainingMonths == 0)
            ? 0m
            : Math.Round(remainingBooks / (decimal)remainingMonths, 1);

        return new GoalProgressResponse(
            goal.Year,
            goal.TargetBooks,
            completed,
            percentage,
            remainingBooks,
            remainingMonths,
            monthlyAverage
        );
    }
}