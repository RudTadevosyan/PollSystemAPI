using Shared.DTOs.Vote;

namespace PollManagement.Domain.Interfaces.ServiceInterfaces;

public interface IVoteService
{
    Task<VoteDto> GetVoteByIdAsync(int pollId, int userId);
    Task<IEnumerable<VoteDto>> GetMyVotesAsync(int userId, int pageNumber = 1, int pageSize = 10);
    Task<VoteDto> AddVoteAsync(CreateVoteDto voteDto, int pollId, int userId);
    Task DeleteVoteAsync(int pollId, int userId);

}