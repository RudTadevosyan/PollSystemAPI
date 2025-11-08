using Shared.DTOs.Poll;

namespace PollManagement.Domain.Interfaces.ServiceInterfaces;

public interface IPollService
{ 
    Task<IEnumerable<PollDto>> GetAllPolls(PollFilterDto filter, bool includeVotes = false, int pageNumber = 1, int pageSize = 10);
    Task<PollDto?> GetPollByIdAsync(int pollId, bool includeVotes = false);
    Task<IEnumerable<PollDto>> GetMyPollsAsync(int userId, bool includeVotes = false, int pageNumber = 1, int pageSize = 10);
    Task<PollDto> CreatePollAsync(CreatePollDto createPollDto, int userId);
    Task UpdatePollAsync(UpdatePollDto updatePollDto, int pollId, int userId);
    Task DeletePollAsync(int id, int userId);
}