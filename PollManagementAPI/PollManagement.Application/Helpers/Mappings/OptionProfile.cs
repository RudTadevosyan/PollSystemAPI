using AutoMapper;
using PollManagement.Domain.Entities;
using Shared.DTOs.Option;

namespace PollManagement.Application.Helpers.Mappings;

public class OptionProfile : Profile
{
    public OptionProfile()
    {
        CreateMap<Option, OptionDto>();
        CreateMap<CreateOptionDto, Option>() 
            .ForMember(dest => dest.VoteCount, opt => opt.MapFrom(src => 0));
        CreateMap<UpdateOptionDto, Option>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
    }
}