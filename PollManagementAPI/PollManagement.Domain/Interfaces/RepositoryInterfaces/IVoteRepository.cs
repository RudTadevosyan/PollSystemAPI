using PollManagement.Domain.Entities;

namespace PollManagement.Domain.Interfaces.RepositoryInterfaces;

public interface IVoteRepository
{ 
    Task<Vote?> GetByUserAndPollAsync(int pollId, int userId);
    Task<IEnumerable<Vote>> GetMyVotesAsync(int userId, int pageNumber = 1, int pageSize = 10);
    Task AddAsync(Vote vote);
    Task DeleteAsync(Vote vote);
}