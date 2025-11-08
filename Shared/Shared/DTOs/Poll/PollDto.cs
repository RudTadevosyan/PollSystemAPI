using Shared.DTOs.Option;
using Shared.DTOs.Vote;

namespace Shared.DTOs.Poll;

public class PollDto
{
    public int PollId { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = null!;
    
    public string? Description { get; set; }
    public DateTime ClosesAt { get; set; }
    public string? Category { get; set; }
    public string? Topic { get; set; }
    public bool Status { get; set; } = true;
    public ICollection<OptionDto> Options { get; set; } = new List<OptionDto>();
    public ICollection<VoteDto> Votes { get; set; } = new List<VoteDto>();
}