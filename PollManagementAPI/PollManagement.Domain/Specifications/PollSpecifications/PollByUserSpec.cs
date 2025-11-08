using System.Linq.Expressions;
using PollManagement.Domain.Entities;
using Shared.Specifications;

namespace PollManagement.Domain.Specifications.PollSpecifications;

public class PollByUserSpec : Specification<Poll>
{
    private readonly int? _userId;

    public PollByUserSpec(int? userId)
    {
        _userId = userId;
    }

    public override Expression<Func<Poll, bool>> ToExpression()
    {
        return p => !_userId.HasValue || 
                    _userId.Value == p.UserId;
    }
}