using System.Linq.Expressions;
using PollManagement.Domain.Entities;
using Shared.DTOs.Poll;
using Shared.Specifications;

namespace PollManagement.Domain.Specifications.PollSpecifications;

public class PollByFilterSpec : Specification<Poll>
{
    private readonly PollFilterDto _filterDto;

    public PollByFilterSpec(PollFilterDto filterDto)
    {
        _filterDto = filterDto;
    }

    public override Expression<Func<Poll, bool>> ToExpression()
    {
        Specification<Poll> spec = new DefaultSpecification<Poll>();
        
        if (!string.IsNullOrWhiteSpace(_filterDto.Category))
            spec = spec.And(new PollByCategorySpec(_filterDto.Category));
        
        if (!string.IsNullOrWhiteSpace(_filterDto.Topic))
            spec = spec.And(new PollByTopicSpec(_filterDto.Topic));
        
        if (_filterDto.Status.HasValue)
            spec = spec.And(new PollByStatusSpec(_filterDto.Status.Value));

        return spec.ToExpression();
    }
}