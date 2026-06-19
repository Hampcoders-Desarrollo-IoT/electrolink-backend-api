using System.Linq.Expressions;

namespace Hampcoders.Electrolink.API.Shared.Domain.Services.Specifications;

public class NotSpecification<T> : ISpecification<T>
{
    private readonly ISpecification<T> _specification;

    public NotSpecification(ISpecification<T> specification)
    {
        _specification = specification;
    }

    public bool IsSatisfiedBy(T entity)
        => !_specification.IsSatisfiedBy(entity);

    public async Task<bool> IsSatisfiedByAsync(T entity)
        => !await _specification.IsSatisfiedByAsync(entity);

    public Expression<Func<T, bool>> ToExpression()
    {
        var expr = _specification.ToExpression();
        var param = expr.Parameters[0];
        var body = Expression.Not(expr.Body);
        return Expression.Lambda<Func<T, bool>>(body, param);
    }
}
