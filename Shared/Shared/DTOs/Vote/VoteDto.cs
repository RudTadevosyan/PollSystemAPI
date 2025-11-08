namespace Shared.DTOs.Vote;

public class VoteDto
{
    public int UserId { get; set; }
    public DateTime VotedAt { get; set; } = DateTime.UtcNow;
    public int OptionId { get; set; }
    public int PollId { get; set; }
}