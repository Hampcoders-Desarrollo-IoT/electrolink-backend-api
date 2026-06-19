namespace Hampcoders.Electrolink.API.Profiles.Domain.Model.Commands;

public record GrantIoTCertificationCommand(
    string TechnicianId,
    string CertificationName,
    string IssuerOrganization,
    DateTime DateObtained,
    DateTime? ExpirationDate,
    string? CredentialId,
    string? CredentialUrl);
