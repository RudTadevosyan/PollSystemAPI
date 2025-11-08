using Shared.DTOs.Poll;

namespace ResultAggregation.Domain.Interfaces;

public interface IPollResultRepository
{
    Task SetPollResultAsync(int pollId, object result);
    Task<T?> GetPollResultAsync<T>(int pollId);
    Task DeletePollResultAsync(int pollId);
}