namespace Shared.DTOs.Option;
public class OptionDto
{
    public int OptionId { get; set; } 
    public string Text { get; set; } = null!;
    public int VoteCount { get; set; } = 0;
    public int PollId { get; set; }
}