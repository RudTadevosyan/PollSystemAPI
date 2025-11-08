using System.Linq.Expressions;

namespace Shared.Specifications;

//for starting point to build with And the specs 
public class DefaultSpecification<T> : Specification<T>
{
    public override Expression<Func<T, bool>> ToExpression() => _ => true;
}