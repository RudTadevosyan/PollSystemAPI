using AutoMapper;
using PollManagement.Domain.Entities;
using PollManagement.Domain.Exceptions;
using PollManagement.Domain.Interfaces.RepositoryInterfaces;
using PollManagement.Domain.Interfaces.ServiceInterfaces;
using Shared.DTOs.Option;

namespace PollManagement.Application.Services;

public class OptionService : IOptionService
{
    private readonly IOptionRepository _optionRepository;
    private readonly IPollRepository _pollRepository;
    private readonly IMapper _mapper;

    public OptionService(IOptionRepository optionRepository, IMapper mapper, IPollRepository pollRepository)
    {
        _optionRepository = optionRepository;
        _mapper = mapper;
        _pollRepository = pollRepository;
    }
    
    public async Task<OptionDto> GetOptionByIdAsync(int pollId, int optionId)
    {
        var option = await _optionRepository.GetByIdAsync(optionId)
                     ?? throw new NotFoundException($"The option with id {optionId} was not found.");
        
        if (option.PollId != pollId)
            throw new DomainException($"The poll with id {pollId} has not option with id {optionId}.");
        
        return _mapper.Map<OptionDto>(option);
    }

    public async Task<OptionDto> AddOptionAsync(CreateOptionDto optionDto, int pollId, int userId)
    {
        var option = _mapper.Map<Option>(optionDto);
        option.PollId = pollId;
        
        if (string.IsNullOrEmpty(optionDto.Text))
            throw new DomainException("Text is required.");
        
        var poll = await _pollRepository.GetByIdAsync(pollId)
            ?? throw new NotFoundException($"The poll with id {pollId} was not found.");
        
        if (poll.UserId != userId)
            throw new DomainException($"The user Id {userId} does not have permission for this poll.");
        
        await _optionRepository.AddAsync(option);
        return _mapper.Map<OptionDto>(option);
    }

    public async Task UpdateOptionAsync(UpdateOptionDto optionDto, int pollId, int optionId, int userId)
    {
        if (string.IsNullOrEmpty(optionDto.Text))
                    throw new DomainException("Text is required.");
        
        var existingOption = await _optionRepository.GetByIdAsync(optionId, true)
            ?? throw new NotFoundException($"The option with id {optionId} was not found.");
        
        if (existingOption.PollId != pollId)
                    throw new DomainException($"The poll with id {pollId} has not option with id {optionId}.");
        
        if(existingOption.Poll.UserId != userId)
            throw new DomainException($"The user Id {userId} does not have permission for this poll.");
        
        _mapper.Map(optionDto, existingOption);
        await _optionRepository.UpdateAsync(existingOption);
    }

    public async Task DeleteOptionAsync(int pollId, int optionId, int userId)
    {
        var existingOption = await _optionRepository.GetByIdAsync(optionId)
                             ?? throw new NotFoundException($"The option with id {optionId} was not found.");
        
        if (existingOption.PollId != pollId)
            throw new DomainException($"The poll with id {pollId} has not option with id {optionId}.");
        
        if(existingOption.Poll.UserId != userId)
            throw new DomainException($"The user Id {userId} does not have permission for this poll.");
        
        
        await _optionRepository.DeleteAsync(existingOption);
    }
}