namespace GCPCostNotifier.Services;

public interface ICostQueryService
{
    public Task<IList<CostSummary>> GetYesterdayCostSummaryAsync(DateTimeOffset targetDateTimeOffset,
        TimeZoneInfo targetTimeZoneInfo,
        CancellationToken cancellationToken);
}
