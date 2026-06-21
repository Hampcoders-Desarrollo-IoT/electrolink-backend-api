using System.Text.Json.Serialization;

namespace Hampcoders.Electrolink.API.Profiles.Domain.Model.ValueObjects;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ECompanySize
{
    Small,
    Medium,
    Large
}
