using Hampcoders.Electrolink.API.Assets.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Repositories;

namespace Hampcoders.Electrolink.API.Assets.Domain.Repositories;

public interface IPropertyRepository : IBaseRepository<Property, PropertyId>
{
    Task<IEnumerable<Property>> FindByOwnerAsync(ClientIdentity owner);
    Task<Property?> FindByIdAndOwnerAsync(PropertyId propertyId, ClientIdentity owner);
    
    Task<IEnumerable<Property>> GetAllFilteredAsync(
        ClientIdentity owner, 
        string? city, 
        string? street
    );

    Task<(IEnumerable<Property> Items, int TotalCount)> GetAllPaginatedAsync(int page, int pageSize);

    Task<(IEnumerable<Property> Items, int TotalCount)> GetAllFilteredPaginatedAsync(
        ClientIdentity owner, string? city, string? street, int page, int pageSize);
}