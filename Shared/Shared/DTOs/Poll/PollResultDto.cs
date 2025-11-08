using Shared.DTOs.Option;
using Shared.DTOs.Vote;

namespace Shared.DTOs.Poll;

public class PollResultDto
{
    public int PollId { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string? Topic { get; set; }
    
    public DateTime ClosedAt { get; set; }
    public ICollection<OptionResultDto> Options { get; set; } = new List<OptionResultDto>(); 
}