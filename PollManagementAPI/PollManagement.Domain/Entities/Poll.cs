namespace PollManagement.Domain.Entities;

public class Poll
{
    public int PollId { get; set; }
    public int UserId { get; set; } // Will take from JWT
    public string Title { get; set; } = null!;
    
    public string? Description { get; set; }
    public DateTime ClosesAt { get; set; }
    public string? Category { get; set; }
    public string? Topic { get; set; }
    public bool Status { get; set; } = true;
    public ICollection<Option> Options { get; set; } = new List<Option>();
    public ICollection<Vote> Votes { get; set; } = new List<Vote>();
}