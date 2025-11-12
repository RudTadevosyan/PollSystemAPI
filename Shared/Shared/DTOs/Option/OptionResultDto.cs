namespace Shared.DTOs.Option;

public class OptionResultDto
{
    public int OptionId { get; set; } 
    public string Text { get; set; } = null!;
    public int VoteCount { get; set; }
    public double Percentage { get; set; }
}