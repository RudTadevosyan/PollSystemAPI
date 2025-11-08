using System.Linq.Expressions;
using PollManagement.Domain.Entities;
using Shared.Specifications;

namespace PollManagement.Domain.Specifications.PollSpecifications;

public class PollByCategorySpec : Specification<Poll>
{
    private readonly string? _category;
    
    public PollByCategorySpec(string? category)
    {
        _category = category?.ToLower();
    }
    public override Expression<Func<Poll, bool>> ToExpression()
    {
        return p => string.IsNullOrEmpty(_category) || 
                    p.Category != null && p.Category.ToLower() == _category;
    }
}