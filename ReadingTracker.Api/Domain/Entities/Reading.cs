using ReadingTracker.Api.Domain.Enums;

namespace ReadingTracker.Api.Domain.Entities;

public class Reading
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid BookId { get; set; }
    public ReadingStatus Status { get; set; } = ReadingStatus.QueroLer;
    public DateOnly? StartedAt { get; set; }
    public DateOnly? FinishedAt { get; set; }

    public User User { get; set; } = null!;
    public Book Book { get; set; } = null!;
    public Review? Review { get; set; }
}