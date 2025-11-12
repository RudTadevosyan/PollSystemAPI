namespace PollManagement.Domain.Entities;

public class Option
{
    public int OptionId { get; set; }
    public string Text { get; set; } = null!;
    public int VoteCount { get; set; }
    public int PollId { get; set; }
    public Poll Poll { get; set; } = null!;

}