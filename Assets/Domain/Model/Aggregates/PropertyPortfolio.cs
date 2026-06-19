using Hampcoders.Electrolink.API.Assets.Domain.Model.Entities;
using Hampcoders.Electrolink.API.Assets.Domain.Model.Events;
using Hampcoders.Electrolink.API.Assets.Domain.Model.ValueObjects;
using Hampcoders.Electrolink.API.Shared.Domain.Model.Aggregates;
using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;
using PropertyId = Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects.PropertyId;

namespace Hampcoders.Electrolink.API.Assets.Domain.Model.Aggregates;

public class PropertyPortfolio : BaseAggregateRoot
{
    public PropertyPortfolioId Id { get; private set; } = null!;
    public ClientIdentity Owner { get; private set; } = null!;

    // ── State ─────────────────────────────────────────────
    public EPortfolioStatus Status  { get; private set; }

    // ── Child collection ──────────────────────────────────
    private readonly List<PortfolioEntry> _entries = new();
    public IReadOnlyCollection<PortfolioEntry> Entries => _entries.AsReadOnly();
    
    public static PropertyPortfolio Create(ClientIdentity owner)
    {
        var portfolio = new PropertyPortfolio
        {
            Id = PropertyPortfolioId.NewPropertyPortfolioId(),
            Owner = owner,
            Status  = EPortfolioStatus.Empty,
        };

        portfolio.RaiseDomainEvent(new PropertyPortfolioCreatedEvent(
            portfolio.Id, portfolio.Owner, DateTime.UtcNow));

        return portfolio;
    }
    
    public void AddProperty(PropertyId propertyId, string nickname, bool isPrimary, EOccupancyStatus occupancyStatus)
    {
        if (_entries.Any(e => e.PropertyId == propertyId))
            throw new InvalidOperationException("Property is already in the portfolio.");
        if (_entries.Any(e => e.Nickname == nickname))
            throw new InvalidOperationException($"Nickname '{nickname}' is already used.");

        if (isPrimary)
            foreach (var existing in _entries)
                existing.SetNonPrimary();

        var entry = PortfolioEntry.Create(Id, propertyId, nickname, isPrimary, occupancyStatus);
        _entries.Add(entry);

        if (Status == EPortfolioStatus.Empty)
            Status = EPortfolioStatus.Active;

        RaiseDomainEvent(new PropertyAddedToPortfolioEvent(
            Id, propertyId, nickname, isPrimary, Status, DateTime.UtcNow));
    }
    
    public void RemoveProperty(PropertyId propertyId, string reason)
    {
        var entry = _entries.FirstOrDefault(e => e.PropertyId == propertyId)
                    ?? throw new KeyNotFoundException($"Property {propertyId.Value} not found in portfolio.");

        bool wasPrimary = entry.IsPrimary;
        _entries.Remove(entry);

        RaiseDomainEvent(new PropertyRemovedFromPortfolioEvent(
            Id, propertyId, reason, wasPrimary, DateTime.UtcNow));
    }
}