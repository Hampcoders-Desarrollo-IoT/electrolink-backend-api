using System.Linq.Expressions;

namespace Hampcoders.Electrolink.API.Shared.Domain.Services.Specifications;

public class AndSpecification<T> : ISpecification<T>
{
    private readonly ISpecification<T> _left;
    private readonly ISpecification<T> _right;

    public AndSpecification(ISpecification<T> left, ISpecification<T> right)
    {
        _left = left;
        _right = right;
    }

    public bool IsSatisfiedBy(T entity)
        => _left.IsSatisfiedBy(entity) && _right.IsSatisfiedBy(entity);

    public async Task<bool> IsSatisfiedByAsync(T entity)
        => await _left.IsSatisfiedByAsync(entity) && await _right.IsSatisfiedByAsync(entity);

    public Expression<Func<T, bool>> ToExpression()
    {
        var leftExpr = _left.ToExpression();
        var rightExpr = _right.ToExpression();
        var param = leftExpr.Parameters[0];
        var body = Expression.AndAlso(leftExpr.Body, Expression.Invoke(rightExpr, param));
        return Expression.Lambda<Func<T, bool>>(body, param);
    }
}
