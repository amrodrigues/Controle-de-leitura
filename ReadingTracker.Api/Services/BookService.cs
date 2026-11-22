using ReadingTracker.Api.Domain.Entities;
using ReadingTracker.Api.Domain.Exceptions;
using ReadingTracker.Api.DTOs;
using ReadingTracker.Api.Infrastructure.Repositories;

namespace ReadingTracker.Api.Services;

public class BookService(IBookRepository books) : IBookService
{
    public async Task<BookResponse> CreateAsync(Guid userId, BookRequest request, CancellationToken ct = default)
    {
        var book = new Book
        {
            UserId = userId,
            Title = request.Title.Trim(),
            Author = request.Author.Trim(),
            PageCount = request.PageCount
        };

        await books.AddAsync(book, ct);
        await books.SaveChangesAsync(ct);

        return ToResponse(book);
    }

    public async Task<List<BookResponse>> GetAllAsync(Guid userId, CancellationToken ct = default)
    {
        var list = await books.GetAllByUserAsync(userId, ct);
        return list.Select(ToResponse).ToList();
    }

    public async Task<BookResponse> UpdateAsync(Guid userId, Guid bookId, BookRequest request, CancellationToken ct = default)
    {
        var book = await books.GetByIdAndUserAsync(bookId, userId, ct)
            ?? throw new NotFoundAppException("Livro não encontrado.");

        book.Title = request.Title.Trim();
        book.Author = request.Author.Trim();
        book.PageCount = request.PageCount;

        await books.SaveChangesAsync(ct);
        return ToResponse(book);
    }

    public async Task DeleteAsync(Guid userId, Guid bookId, CancellationToken ct = default)
    {
        var book = await books.GetByIdAndUserAsync(bookId, userId, ct)
            ?? throw new NotFoundAppException("Livro não encontrado.");

        if (await books.IsInShelfAsync(bookId, userId, ct))
            throw new ConflictAppException("Não é possível excluir um livro que está na estante. Retire-o da estante antes.");

        books.Delete(book);
        await books.SaveChangesAsync(ct);
    }

    private static BookResponse ToResponse(Book b) => new(b.Id, b.Title, b.Author, b.PageCount);
}