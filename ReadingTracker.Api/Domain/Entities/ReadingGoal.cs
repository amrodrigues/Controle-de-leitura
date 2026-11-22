namespace ReadingTracker.Api.Domain.Entities;

public class ReadingGoal
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public int Year { get; set; }
    public int TargetBooks { get; set; }

    public User User { get; set; } = null!;
}