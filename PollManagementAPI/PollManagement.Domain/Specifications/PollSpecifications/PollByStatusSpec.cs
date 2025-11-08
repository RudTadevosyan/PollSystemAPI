using System.Linq.Expressions;
using PollManagement.Domain.Entities;
using Shared.Specifications;

namespace PollManagement.Domain.Specifications.PollSpecifications;

public class PollByStatusSpec : Specification<Poll>
{
    private readonly bool? _status;

    public PollByStatusSpec(bool? status)
    {
        _status = status;
    }
    public override Expression<Func<Poll, bool>> ToExpression()
    {
        return p => !_status.HasValue || p.Status == _status;
    }
}