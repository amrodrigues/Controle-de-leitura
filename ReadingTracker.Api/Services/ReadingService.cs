using ReadingTracker.Api.Domain.Entities;
using ReadingTracker.Api.Domain.Enums;
using ReadingTracker.Api.Domain.Exceptions;
using ReadingTracker.Api.DTOs;
using ReadingTracker.Api.Infrastructure.Repositories;

namespace ReadingTracker.Api.Services;

public class ReadingService(
    IReadingRepository readings,
    IBookRepository books,
    TimeProvider timeProvider) : IReadingService
{
    public async Task<ReadingResponse> AddToShelfAsync(Guid userId, AddReadingRequest request, CancellationToken ct = default)
    {
        var book = await books.GetByIdAndUserAsync(request.BookId, userId, ct)
            ?? throw new NotFoundAppException("Livro não encontrado.");

        if (await readings.ExistsBookInShelfAsync(request.BookId, userId, ct))
            throw new ConflictAppException("Este livro já está na sua estante.", "BookId");

        var reading = new Reading
        {
            UserId = userId,
            BookId = book.Id,
            Book = book,
            Status = ReadingStatus.QueroLer
        };

        await readings.AddAsync(reading, ct);
        await readings.SaveChangesAsync(ct);

        return ToResponse(reading);
    }

    public async Task<List<ReadingResponse>> GetShelfAsync(Guid userId, ReadingStatus? status, int? year, CancellationToken ct = default)
    {
        var list = await readings.GetByUserAsync(userId, status, year, ct);
        return list.Select(ToResponse).ToList();
    }

    public async Task<ReadingResponse> UpdateStatusAsync(Guid userId, Guid readingId, UpdateReadingStatusRequest request, CancellationToken ct = default)
    {
        var reading = await readings.GetByIdAndUserAsync(readingId, userId, ct)
            ?? throw new NotFoundAppException("Leitura não encontrada na estante.");

        // Regra 1: ordem das etapas QueroLer -> Lendo -> Lido (sem pular e sem voltar)
        if (reading.Status == ReadingStatus.Lido)
            throw new ValidationRuleException("Uma leitura concluída (Lido) não pode voltar para etapas anteriores. Para reler, crie uma nova leitura.", "Status");

        if (reading.Status == ReadingStatus.QueroLer && request.Status != ReadingStatus.Lendo)
            throw new ValidationRuleException("Não é permitido pular etapas. De 'QueroLer' o livro deve passar primeiro para 'Lendo'.", "Status");

        if (reading.Status == ReadingStatus.Lendo && request.Status != ReadingStatus.Lido)
            throw new ValidationRuleException("Transição de status inválida. Uma leitura em 'Lendo' só pode avançar para 'Lido'.", "Status");

        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);

        if (request.Status == ReadingStatus.Lendo)
        {
            reading.Status = ReadingStatus.Lendo;
            reading.StartedAt = request.StartedAt ?? today;
        }
        else if (request.Status == ReadingStatus.Lido)
        {
            // Regra 2: Lido exige data de término e término >= início
            if (!request.FinishedAt.HasValue)
                throw new ValidationRuleException("Ao marcar a leitura como 'Lido', a data de término é obrigatória.", "FinishedAt");

            var effectiveStart = request.StartedAt ?? reading.StartedAt;
            if (effectiveStart.HasValue && request.FinishedAt.Value < effectiveStart.Value)
                throw new ValidationRuleException("A data de término não pode ser anterior à data de início.", "FinishedAt");

            if (request.StartedAt.HasValue)
                reading.StartedAt = request.StartedAt;

            reading.FinishedAt = request.FinishedAt.Value;
            reading.Status = ReadingStatus.Lido;
        }

        await readings.SaveChangesAsync(ct);
        return ToResponse(reading);
    }

    public async Task<ReadingResponse> ReReadAsync(Guid userId, Guid bookId, CancellationToken ct = default)
    {
        var book = await books.GetByIdAndUserAsync(bookId, userId, ct)
            ?? throw new NotFoundAppException("Livro não encontrado.");

        var userReadings = await readings.GetByUserAsync(userId, null, null, ct);
        var bookReadings = userReadings.Where(r => r.BookId == bookId).ToList();

        if (bookReadings.Count == 0)
            throw new ValidationRuleException("Adicione o livro à estante antes de registrar uma releitura.");

        if (bookReadings.Any(r => r.Status != ReadingStatus.Lido))
            throw new ConflictAppException("Já existe uma leitura em andamento ou planejada para este livro.");

        var newReading = new Reading
        {
            UserId = userId,
            BookId = book.Id,
            Book = book,
            Status = ReadingStatus.QueroLer
        };

        await readings.AddAsync(newReading, ct);
        await readings.SaveChangesAsync(ct);

        return ToResponse(newReading);
    }

    public async Task RemoveFromShelfAsync(Guid userId, Guid readingId, CancellationToken ct = default)
    {
        var reading = await readings.GetByIdAndUserAsync(readingId, userId, ct)
            ?? throw new NotFoundAppException("Leitura não encontrada na estante.");

        readings.Delete(reading);
        await readings.SaveChangesAsync(ct);
    }

    public async Task<ReviewResponse> UpsertReviewAsync(Guid userId, Guid readingId, UpsertReviewRequest request, CancellationToken ct = default)
    {
        var reading = await readings.GetByIdAndUserAsync(readingId, userId, ct)
            ?? throw new NotFoundAppException("Leitura não encontrada na estante.");

        // Regra 3: Só se avalia o que foi concluído
        if (reading.Status != ReadingStatus.Lido)
            throw new ValidationRuleException("Apenas leituras concluídas (Lido) podem receber avaliação.", "Status");

        if (reading.Review is null)
        {
            var review = new Review
            {
                ReadingId = reading.Id,
                UserId = userId,
                Rating = request.Rating,
                Comment = string.IsNullOrWhiteSpace(request.Comment) ? null : request.Comment.Trim(),
                UpdatedAt = timeProvider.GetUtcNow()
            };
            await readings.AddReviewAsync(review, ct);
            await readings.SaveChangesAsync(ct);
            return new ReviewResponse(review.Id, review.ReadingId, review.Rating, review.Comment, review.UpdatedAt);
        }

        reading.Review.Rating = request.Rating;
        reading.Review.Comment = string.IsNullOrWhiteSpace(request.Comment) ? null : request.Comment.Trim();
        reading.Review.UpdatedAt = timeProvider.GetUtcNow();

        await readings.SaveChangesAsync(ct);
        return new ReviewResponse(reading.Review.Id, reading.Review.ReadingId, reading.Review.Rating, reading.Review.Comment, reading.Review.UpdatedAt);
    }

    private static ReadingResponse ToResponse(Reading r) => new(
        r.Id,
        r.BookId,
        r.Book.Title,
        r.Book.Author,
        r.Book.PageCount,
        r.Status,
        r.StartedAt,
        r.FinishedAt,
        r.Review is null ? null : new ReviewResponse(r.Review.Id, r.Review.ReadingId, r.Review.Rating, r.Review.Comment, r.Review.UpdatedAt)
    );
}