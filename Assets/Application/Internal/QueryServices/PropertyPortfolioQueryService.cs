using Hampcoders.Electrolink.API.Assets.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Assets.Domain.Model.Queries;
using Hampcoders.Electrolink.API.Assets.Domain.Repositories;
using Hampcoders.Electrolink.API.Assets.Domain.Services;
using Hampcoders.Electrolink.API.Shared.Domain.Repositories;
using MediatR;

namespace Hampcoders.Electrolink.API.Assets.Application.Internal.QueryServices;

public class PropertyPortfolioQueryService(IPropertyPortfolioRepository propertyPortfolioRepository, IUnitOfWork unitOfWork, IMediator mediator) : IPropertyPortfolioQueryService
{
    public async Task<PropertyPortfolio?> Handle(GetPortfolioByOwnerIdQuery query)
        => await propertyPortfolioRepository.FindByOwnerWithEntriesAsync(query.Owner);
}