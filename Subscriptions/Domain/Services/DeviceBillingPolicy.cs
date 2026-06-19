namespace Hampcoders.Electrolink.API.Subscriptions.Domain.Services;

public static class DeviceBillingPolicy
{
    public static int CalculateMonthlyTotal(int activeDeviceCount, int pricePerDevice)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(activeDeviceCount);
        ArgumentOutOfRangeException.ThrowIfNegative(pricePerDevice);
        return activeDeviceCount * pricePerDevice;
    }

    public const int MinimumDeviceCount = 5;

    public const int MaximumDeviceCountForStandardPricing = 500;
}
