namespace Shared.DTOs.Poll;

public class CreatePollDto
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime ClosesAt { get; set; }
    public string? Category { get; set; }
    public string? Topic { get; set; }
}