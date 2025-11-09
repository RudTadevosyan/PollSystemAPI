using AutoMapper;
using PollManagement.Domain.Entities;
using PollManagement.Domain.Exceptions;
using PollManagement.Domain.Interfaces.RepositoryInterfaces;
using PollManagement.Domain.Interfaces.ServiceInterfaces;
using PollManagement.Domain.Specifications.PollSpecifications;
using Shared.DTOs.Poll;

namespace PollManagement.Application.Services;

public class PollService : IPollService
{
    private readonly IPollRepository _pollRepository;
    private readonly IMapper _mapper;

    public PollService(IPollRepository pollRepository, IMapper mapper)
    {
        _pollRepository = pollRepository;
        _mapper = mapper;
    }
    public async Task<IEnumerable<PollDto>> GetAllPolls(PollFilterDto filter, bool includeVotes = false, int pageNumber = 1, int pageSize = 10)
    {
        if (pageNumber <= 0 || pageNumber >= 100) pageNumber = 1;
        if (pageSize <= 0 || pageSize >= 100) pageSize = 10;
        
        var spec = new PollByFilterSpec(filter);
        var items = await _pollRepository.GetAllAsync(spec, includeVotes ,pageNumber, pageSize);
        
        return _mapper.Map<IEnumerable<PollDto>>(items);
    }
    public async Task<PollDto?> GetPollByIdAsync(int id, bool includeVotes = false)
    {
        var poll = await _pollRepository.GetByIdAsync(id, includeVotes)
            ?? throw new NotFoundException($"Poll with the Id {id} was not found");
        
        return _mapper.Map<PollDto>(poll);
    }

    public async Task<IEnumerable<PollDto>> GetMyPollsAsync(int userId, bool includeVotes = false, int pageNumber = 1, int pageSize = 10)
    {
        if (pageNumber <= 0 || pageNumber >= 100) pageNumber = 1;
        if (pageSize <= 0 || pageSize >= 100) pageSize = 10;
        
        var items = await _pollRepository.GetMyPollsAsync(userId, includeVotes, pageNumber, pageSize);
        return _mapper.Map<IEnumerable<PollDto>>(items);
    }
    
    public async Task<PollDto> CreatePollAsync(CreatePollDto createPollDto, int userId)
    {
        var poll = _mapper.Map<Poll>(createPollDto);
        poll.UserId = userId;
        
        if (string.IsNullOrEmpty(createPollDto.Title))
            throw new DomainException("Title is required");
        
        if (createPollDto.ClosesAt <= DateTime.UtcNow)
            throw new DomainException("ClosesAt cant be in the past");
        
        await _pollRepository.AddAsync(poll);
        
        return _mapper.Map<PollDto>(poll);
    }

    public async Task UpdatePollAsync(UpdatePollDto updatePollDto, int pollId, int userId)
    {
        var poll = await _pollRepository.GetByIdAsync(pollId)
            ?? throw new NotFoundException($"Poll with the Id {pollId} was not found");
        
        if (poll.UserId != userId)
            throw new DomainException($"User with id {userId} does not have permissions to this poll"); 
        
        if (updatePollDto.ClosesAt <= DateTime.UtcNow)
            throw new DomainException("ClosesAt cant be in the past");
        
        _mapper.Map(updatePollDto, poll);
        await _pollRepository.UpdateAsync(poll);
    }

    public async Task DeletePollAsync(int id, int userId)
    {
        var poll = await _pollRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Poll with the Id {id} was not found");
        if (poll.UserId != userId)
            throw new DomainException($"User with id {userId} does not have permissions for this poll");
        
        await _pollRepository.DeleteAsync(poll);
    }
}