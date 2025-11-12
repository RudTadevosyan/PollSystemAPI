using Shared.DTOs.Poll;

namespace ResultAggregation.Domain.Interfaces;

public interface IPollResultService
{
    Task<PollResultDto?> GetPollResult(int pollId);
}