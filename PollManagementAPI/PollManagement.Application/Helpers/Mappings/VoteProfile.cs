using AutoMapper;
using PollManagement.Domain.Entities;
using Shared.DTOs.Vote;

namespace PollManagement.Application.Helpers.Mappings;

public class VoteProfile : Profile
{
    public VoteProfile()
    {
        CreateMap<Vote, VoteDto>();
        CreateMap<CreateVoteDto, Vote>();
    }
}

