using AutoMapper;
using PollManagement.Domain.Entities;
using Shared.DTOs.Poll;

namespace PollManagement.Application.Helpers.Mappings;

public class PollProfile : Profile
{
    public PollProfile()
    {
        CreateMap<Poll, PollDto>()
            .ForMember(dest => dest.Votes, opt => opt.MapFrom(src => src.Votes))
            .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options));
        
        // This ensures nulls in DTO don’t overwrite existing DB values.
        CreateMap<UpdatePollDto, Poll>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) =>
            {
                if (srcMember == null) return false;
                if(srcMember is DateTime dt && dt == default) return false; // as DateTime is value type if nothing passed will take default value
                return true;
            }));
        CreateMap<CreatePollDto, Poll>();
    }
}