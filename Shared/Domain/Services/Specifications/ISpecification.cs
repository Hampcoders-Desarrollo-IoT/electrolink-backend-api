using System.Linq.Expressions;

namespace Hampcoders.Electrolink.API.Shared.Domain.Services.Specifications;

public interface ISpecification<T>
{
    bool IsSatisfiedBy(T entity);
    Task<bool> IsSatisfiedByAsync(T entity);
    Expression<Func<T, bool>> ToExpression();
}
