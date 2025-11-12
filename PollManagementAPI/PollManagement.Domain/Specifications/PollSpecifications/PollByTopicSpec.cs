using System.Linq.Expressions;
using PollManagement.Domain.Entities;
using Shared.Specifications;

namespace PollManagement.Domain.Specifications.PollSpecifications;

public class PollByTopicSpec : Specification<Poll>
{
    private readonly string? _topic;

    public PollByTopicSpec(string? topic)
    {
        _topic = topic?.ToLower();
    }

    public override Expression<Func<Poll, bool>> ToExpression()
    {
        return p => string.IsNullOrEmpty(_topic) || 
                    p.Topic != null && p.Topic.ToLower() == _topic;
    }
}