namespace PollManagement.Domain.Entities;

public class Vote
{
    public int UserId { get; set; }
    public DateTime VotedAt { get; set; } = DateTime.UtcNow;

    public int OptionId { get; set; }
    public Option Option { get; set; } = null!;
    
    public int PollId { get; set; }
    public Poll Poll { get; set; } = null!;
}