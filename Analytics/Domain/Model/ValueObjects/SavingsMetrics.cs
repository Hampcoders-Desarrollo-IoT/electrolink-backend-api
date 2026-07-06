namespace Hampcoders.Electrolink.API.Analytics.Domain.Model.ValueObjects;

public record SavingsMetrics
{
    public decimal PreventiveCostPerEvent { get; init; }
    public decimal CorrectiveCostPerEvent { get; init; }
    public int AnomaliesDetectedOnTime { get; init; }
    public int AnomaliesDetectedLate { get; init; }

    public decimal EstimatedSavingsPerEvent =>
        CorrectiveCostPerEvent - PreventiveCostPerEvent;

    public decimal TotalEstimatedSavings =>
        AnomaliesDetectedOnTime * EstimatedSavingsPerEvent;

    public int TotalAnomalies =>
        AnomaliesDetectedOnTime + AnomaliesDetectedLate;

    private SavingsMetrics() { }

    public static SavingsMetrics Create(
        decimal preventiveCost = 2000m,
        decimal correctiveCost = 8000m,
        int anomaliesOnTime = 0,
        int anomaliesLate = 0)
    {
        if (preventiveCost <= 0)
            throw new ArgumentException("Preventive cost must be positive.");
        if (correctiveCost <= preventiveCost)
            throw new ArgumentException("Corrective cost must be greater than preventive cost.");

        return new SavingsMetrics
        {
            PreventiveCostPerEvent = preventiveCost,
            CorrectiveCostPerEvent = correctiveCost,
            AnomaliesDetectedOnTime = anomaliesOnTime,
            AnomaliesDetectedLate = anomaliesLate
        };
    }

    public SavingsMetrics RecordAnomalyDetectedOnTime() =>
        this with { AnomaliesDetectedOnTime = AnomaliesDetectedOnTime + 1 };

    public SavingsMetrics RecordAnomalyDetectedLate() =>
        this with { AnomaliesDetectedLate = AnomaliesDetectedLate + 1 };
}
