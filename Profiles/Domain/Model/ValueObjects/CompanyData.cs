using Hampcoders.Electrolink.API.Shared.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Profiles.Domain.Model.ValueObjects;

public sealed record CompanyData
{
    public string CompanyName { get; init; }
    public TaxId TaxId { get; init; }
    public string? Industry { get; init; }
    public ECompanySize Size { get; init; }
    public string? Website { get; init; }
    public Address BillingAddress { get; init; }

    private CompanyData() { }

    public static CompanyData Create(
        string companyName,
        TaxId taxId,
        Address billingAddress,
        string? industry = null,
        ECompanySize size = ECompanySize.Small,
        string? website = null)
    {
        if (string.IsNullOrWhiteSpace(companyName))
            throw new ArgumentException("Company name is required.", nameof(companyName));

        return new CompanyData
        {
            CompanyName = companyName.Trim(),
            TaxId = taxId,
            Industry = industry?.Trim(),
            Size = size,
            Website = website?.Trim(),
            BillingAddress = billingAddress
        };
    }

    public CompanyData Update(
        string? companyName = null,
        string? industry = null,
        ECompanySize? size = null,
        string? website = null,
        Address? billingAddress = null) =>
        this with
        {
            CompanyName = companyName?.Trim() ?? CompanyName,
            Industry = industry?.Trim() ?? Industry,
            Size = size ?? Size,
            Website = website?.Trim() ?? Website,
            BillingAddress = billingAddress ?? BillingAddress,
        };
}
