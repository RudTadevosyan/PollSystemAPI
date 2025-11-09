using AutoMapper;
using PollManagement.Domain.Entities;
using PollManagement.Domain.Exceptions;
using PollManagement.Domain.Interfaces.RepositoryInterfaces;
using PollManagement.Domain.Interfaces.ServiceInterfaces;
using Shared.DTOs.Vote;

namespace PollManagement.Application.Services;

public class VoteService : IVoteService
{
    private readonly IVoteRepository _voteRepository;
    private readonly IMapper _mapper;
    
    public VoteService(IVoteRepository voteRepository, IMapper mapper)
    {
        _voteRepository = voteRepository;
        _mapper = mapper;
    }

    public async Task<VoteDto> GetVoteByIdAsync(int pollId, int userId)
    {
        var vote = await _voteRepository.GetByUserAndPollAsync(pollId, userId) 
                   ?? throw new NotFoundException($"Vote with the Poll Id {pollId} and User Id {userId} was not found");
        
        return _mapper.Map<VoteDto>(vote);
    }

    public async Task<IEnumerable<VoteDto>> GetMyVotesAsync(int userId, int pageNumber = 1, int pageSize = 10)
    {
        if (pageNumber <= 0 || pageNumber >= 100) pageNumber = 1;
        if (pageSize <= 0 || pageSize >= 100) pageSize = 10;
        
        var votes = await _voteRepository.GetMyVotesAsync(userId, pageNumber, pageSize);
        return _mapper.Map<IEnumerable<VoteDto>>(votes);
    }
    
    public async Task<VoteDto> AddVoteAsync(CreateVoteDto voteDto, int pollId, int userId)
    {
        var vote = _mapper.Map<Vote>(voteDto);
        vote.UserId = userId;
        vote.PollId = pollId;

        await _voteRepository.AddAsync(vote);

        return _mapper.Map<VoteDto>(vote);
    }

    public async Task DeleteVoteAsync(int pollId, int userId)
    {
        var existing = await _voteRepository.GetByUserAndPollAsync(pollId, userId)
            ?? throw new NotFoundException($"Vote with the Poll Id {pollId} and User Id {userId} was not found");
        
        if (existing.UserId != userId)
            throw new DomainException($"User with id {userId} does not have permissions for this vote.");
        
        await _voteRepository.DeleteAsync(existing);
    }
}