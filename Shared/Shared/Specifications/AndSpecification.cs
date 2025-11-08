using System.Linq.Expressions;

namespace Shared.Specifications;

public class AndSpecification<T> : Specification<T>
{
    private readonly Specification<T> _left;
    private readonly Specification<T> _right;

    public AndSpecification(Specification<T> left, Specification<T> right)
    {
        _left = left;
        _right = right;
    }
    public override Expression<Func<T, bool>> ToExpression()
    {
        var leftExpr = _left.ToExpression();
        var rightExpr = _right.ToExpression();
        
        var param = Expression.Parameter(typeof(T), "x");
        var left = Expression.Invoke(leftExpr, param);
        var right = Expression.Invoke(rightExpr, param);
        var andExpr = Expression.AndAlso(left, right);
        
        return Expression.Lambda<Func<T, bool>>(andExpr, param);
        
    }
}