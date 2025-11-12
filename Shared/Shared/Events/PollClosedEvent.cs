using Shared.DTOs.Poll;

namespace Shared.Events;

public class PollClosedEvent
{
    public required PollResultDto PollResult { get; set; }
}