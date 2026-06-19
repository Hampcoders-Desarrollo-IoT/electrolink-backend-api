using System.Collections.Generic;

namespace Hampcoders.Electrolink.API.Profiles.Interfaces.REST.Resources;

public record SetThresholdsResource(Dictionary<string, decimal> Thresholds);
