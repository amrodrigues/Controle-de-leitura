namespace ReadingTracker.Api.Domain.Entities;

public class Book
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int PageCount { get; set; }

    public User User { get; set; } = null!;
    public ICollection<Reading> Readings { get; set; } = new List<Reading>();
}