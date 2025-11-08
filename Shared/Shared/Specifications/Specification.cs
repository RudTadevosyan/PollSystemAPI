using System.Linq.Expressions;

namespace Shared.Specifications;

public abstract class Specification<T>
{
    public abstract Expression<Func<T, bool>> ToExpression();

    public Specification<T> And(Specification<T> other)
    {
        // make new Expression with both
        return new AndSpecification<T>(this, other);
    }
}
