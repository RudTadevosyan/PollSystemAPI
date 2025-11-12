namespace Shared.Events;

public record GetVotersResponse(int PollId, List<int> UserIds);