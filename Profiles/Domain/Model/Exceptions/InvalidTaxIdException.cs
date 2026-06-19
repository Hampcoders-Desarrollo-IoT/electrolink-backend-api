using System.Runtime.Serialization;

namespace Hampcoders.Electrolink.API.Profiles.Domain.Model.Exceptions;

[Serializable]
public sealed class InvalidTaxIdException : Exception
{
    public string TaxId { get; }

    public InvalidTaxIdException(string? taxId)
        : base(taxId is null ? "A tax ID is required." : $"The tax ID '{taxId}' is invalid.")
    {
        TaxId = taxId ?? string.Empty;
    }

    public InvalidTaxIdException(string? message, Exception? innerException = null)
        : base(message, innerException)
    {
        TaxId = string.Empty;
    }
}
