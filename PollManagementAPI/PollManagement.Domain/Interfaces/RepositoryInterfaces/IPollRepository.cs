using PollManagement.Domain.Entities;
using Shared.Specifications;

namespace PollManagement.Domain.Interfaces.RepositoryInterfaces;

public interface IPollRepository
{
    Task<IEnumerable<Poll>> GetAllAsync(Specification<Poll> spec, bool includeVotes = false, int pageNumber = 1, int pageSize = 10);
    Task<Poll?> GetByIdAsync(int pollId, bool includeVotes = false);
    Task<IEnumerable<Poll>> GetMyPollsAsync(int userId, bool includeVotes = false, int pageNumber = 1, int pageSize = 10);
    Task AddAsync(Poll poll);
    Task UpdateAsync(Poll poll);
    Task DeleteAsync(Poll poll);
}
