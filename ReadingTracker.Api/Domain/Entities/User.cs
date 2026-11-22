namespace ReadingTracker.Api.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<Book> Books { get; set; } = new List<Book>();
    public ICollection<Reading> Readings { get; set; } = new List<Reading>();
    public ICollection<ReadingGoal> Goals { get; set; } = new List<ReadingGoal>();
}