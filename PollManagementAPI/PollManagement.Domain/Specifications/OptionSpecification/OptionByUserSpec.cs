using System.Linq.Expressions;
using PollManagement.Domain.Entities;
using Shared.Specifications;

namespace PollManagement.Domain.Specifications.OptionSpecification;

public class OptionByUserSpec : Specification<Option>
{
    private readonly int? _userId;

    public OptionByUserSpec(int? userId)
    {
        _userId = userId;
    }

    public override Expression<Func<Option, bool>> ToExpression()
    {
        return o => !_userId.HasValue || 
                    o.Poll.UserId == _userId;
    }
}