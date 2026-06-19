using Hampcoders.Electrolink.API.Shared.Domain.Model.Exceptions;

namespace Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

public record ClientIdentity
{
    public EClientType ClientType { get; init; }
    public string ClientId { get; init; }

    private ClientIdentity(EClientType clientType, string clientId)
    {
        ClientType = clientType;
        ClientId = clientId;
    }

    public static ClientIdentity FromHomeowner(HomeownerId id)
        => new(EClientType.Homeowner, id.Value);

    public static ClientIdentity FromHomeowner(string value)
        => new(EClientType.Homeowner, HomeownerId.From(value).Value);

    public static ClientIdentity FromCompany(CompanyId id)
        => new(EClientType.Company, id.Value);

    public static ClientIdentity FromCompany(string value)
        => new(EClientType.Company, CompanyId.From(value).Value);

    public static ClientIdentity From(string clientType, string value)
    {
        var type = clientType?.Trim().ToUpperInvariant() switch
        {
            "HOMEOWNER" => EClientType.Homeowner,
            "COMPANY" => EClientType.Company,
            _ => throw new ArgumentException($"Invalid client type: {clientType}")
        };
        return new ClientIdentity(type, value);
    }

    public HomeownerId ToHomeownerId()
    {
        if (ClientType != EClientType.Homeowner)
            throw new InvalidOperationException($"ClientIdentity is not a Homeowner (type={ClientType})");
        return HomeownerId.From(ClientId);
    }

    public CompanyId ToCompanyId()
    {
        if (ClientType != EClientType.Company)
            throw new InvalidOperationException($"ClientIdentity is not a Company (type={ClientType})");
        return CompanyId.From(ClientId);
    }

    public bool IsHomeowner => ClientType == EClientType.Homeowner;
    public bool IsCompany => ClientType == EClientType.Company;

    public override string ToString() => $"{ClientType}:{ClientId}";
}
