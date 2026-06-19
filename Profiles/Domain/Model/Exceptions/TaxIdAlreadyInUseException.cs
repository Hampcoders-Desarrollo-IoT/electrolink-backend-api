using System.Runtime.Serialization;
using Hampcoders.Electrolink.API.Profiles.Domain.Model.ValueObjects;

namespace Hampcoders.Electrolink.API.Profiles.Domain.Model.Exceptions;

[Serializable]
public sealed class TaxIdAlreadyInUseException : Exception
{
    public string TaxId { get; }

    public TaxIdAlreadyInUseException(TaxId? taxId)
        : base(taxId is null ? "A Tax ID is already in use." : $"The Tax ID '{taxId}' is already in use.")
    {
        TaxId = taxId?.ToString() ?? string.Empty;
    }

    public TaxIdAlreadyInUseException(string? message, Exception? innerException = null)
        : base(message, innerException)
    {
        TaxId = string.Empty;
    }
}
