namespace Hampcoders.Electrolink.API.Analytics.Interfaces.REST.Resources;

public record AlertLogResource(
    string LogId,
    string OwnerId,
    List<AlertEntryResource> Entries);
