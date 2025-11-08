namespace Shared.DTOs.Poll;

public class UpdatePollDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? ClosesAt { get; set; }
    public string? Category { get; set; }
    public string? Topic { get; set; }
}